using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Infrastructure.Data;
using System.Security.Claims;
using System.Linq;

namespace Accounting.API.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip audit for certain paths
            if (ShouldSkipAudit(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var originalBodyStream = context.Response.Body;
            var requestBody = await GetRequestBody(context.Request);

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                var responseBodyContent = await GetResponseBody(context.Response);

                // Log the audit trail
                await LogAuditTrail(context, requestBody, responseBodyContent);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in audit middleware");
                await responseBody.CopyToAsync(originalBodyStream);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool ShouldSkipAudit(PathString path)
        {
            var skipPaths = new[]
            {
                "/swagger",
                "/health",
                "/uploads",
                "/api/auth/refresh",
                "/favicon.ico"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<string> GetRequestBody(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0)
                return string.Empty;

            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            return body;
        }

        private async Task<string> GetResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }

        private async Task LogAuditTrail(HttpContext context, string requestBody, string responseBody)
        {
            try
            {
                using var scope = context.RequestServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();

                var auditLog = new AuditLog();
                auditLog.SetHttpContext(
                    context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    context.Request.Headers["User-Agent"].ToString() ?? "Unknown"
                );

                // Determine action based on HTTP method and response status
                var action = DetermineAction(context.Request.Method, context.Response.StatusCode);
                if (action == null) return; // Skip if no relevant action

                auditLog.Action = action.Value;
                auditLog.EntityName = ExtractEntityName(context.Request.Path);
                auditLog.EntityId = ExtractEntityId(context.Request.Path, responseBody);

                // Set request/response data
                if (!string.IsNullOrEmpty(requestBody))
                {
                    auditLog.NewValues = requestBody;
                }

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    auditLog.Changes = $"Operation completed successfully. Status: {context.Response.StatusCode}";
                }
                else
                {
                    auditLog.Changes = $"Operation failed. Status: {context.Response.StatusCode}";
                    auditLog.AdditionalInfo = responseBody;
                }

                // Get user ID from claims
                var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    auditLog.UserId = userId;
                }

                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit trail");
            }
        }

        private AuditAction? DetermineAction(string httpMethod, int statusCode)
        {
            // Only log successful operations
            if (statusCode < 200 || statusCode >= 300)
                return null;

            return httpMethod.ToUpper() switch
            {
                "POST" => AuditAction.Create,
                "PUT" => AuditAction.Update,
                "PATCH" => AuditAction.Update,
                "DELETE" => AuditAction.Delete,
                _ => null // Don't audit GET requests by default
            };
        }

        private string ExtractEntityName(PathString path)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments?.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return segments[1]; // Return the controller name
            }
            return "Unknown";
        }

        private string ExtractEntityId(PathString path, string responseBody)
        {
            var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            // Try to get ID from URL path (e.g., /api/banks/123)
            if (segments?.Length >= 3 && int.TryParse(segments[2], out _))
            {
                return segments[2];
            }

            // Try to extract ID from response body for CREATE operations
            try
            {
                if (!string.IsNullOrEmpty(responseBody))
                {
                    dynamic response = JsonConvert.DeserializeObject(responseBody);
                    if (response?.data?.id != null)
                    {
                        return response.data.id.ToString();
                    }
                    if (response?.id != null)
                    {
                        return response.id.ToString();
                    }
                }
            }
            catch
            {
                // Ignore JSON parsing errors
            }

            return null;
        }
    }
}