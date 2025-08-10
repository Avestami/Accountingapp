using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Accounting.Infrastructure.Data;
using Accounting.Application.Interfaces;
using Accounting.Application.Features.Tickets.Handlers;
using Accounting.Infrastructure;
using Accounting.Infrastructure.Repositories;
using Accounting.Application.Common.Authorization;
using Accounting.Application.Services;
using Accounting.Application.Common.Exceptions;
using Accounting.API.Middleware;

namespace Accounting.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Database
            services.AddDbContext<AccountingDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register DbContext interface
            services.AddScoped<IAccountingDbContext>(provider => provider.GetService<AccountingDbContext>());

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IVoucherRepository, VoucherRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register services
            services.AddScoped<IDocumentNumberService, DocumentNumberService>();
            services.AddScoped<IFxFifoService, FxFifoService>();

            // Register Report Services
            services.AddScoped<Accounting.Application.Features.Reports.Services.IReportGenerationService, Accounting.Application.Features.Reports.Services.ReportGenerationService>();
            services.AddScoped<Accounting.Application.Features.Reports.Services.IExportService, Accounting.Application.Features.Reports.Services.ExportService>();

            // Register Query Handlers
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Dashboard.Queries.GetDashboardStatsQuery, Accounting.Application.Common.Models.Result<Accounting.Application.Features.Dashboard.Queries.DashboardStatsDto>>, Accounting.Application.Features.Dashboard.Queries.GetDashboardStatsQueryHandler>();

            // Register Airlines Handlers
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Airlines.Commands.CreateAirlineCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.AirlineDto>>, Accounting.Application.Features.Airlines.Handlers.CreateAirlineCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Airlines.Commands.UpdateAirlineCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.AirlineDto>>, Accounting.Application.Features.Airlines.Handlers.UpdateAirlineCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Airlines.Commands.DeleteAirlineCommand, Accounting.Application.Common.Models.Result<bool>>, Accounting.Application.Features.Airlines.Handlers.DeleteAirlineCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Airlines.Queries.GetAirlinesQuery, Accounting.Application.Common.Models.Result<Accounting.Application.Common.Models.PagedResult<Accounting.Application.DTOs.AirlineDto>>>, Accounting.Application.Features.Airlines.Handlers.GetAirlinesQueryHandler>();
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Airlines.Queries.GetAirlineByIdQuery, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.AirlineDto>>, Accounting.Application.Features.Airlines.Handlers.GetAirlineByIdQueryHandler>();

            // Register Voucher Handlers
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Vouchers.Queries.GetVouchersQuery, Accounting.Application.Common.Models.Result<Accounting.Application.Common.Models.PagedResult<Accounting.Application.DTOs.VoucherDto>>>, Accounting.Application.Features.Vouchers.Handlers.GetVouchersQueryHandler>();
            services.AddScoped<Accounting.Application.Common.Queries.IQueryHandler<Accounting.Application.Features.Vouchers.Queries.GetVoucherByIdQuery, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.GetVoucherByIdQueryHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.CreateVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.CreateVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.UpdateVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.UpdateVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.DeleteVoucherCommand, Accounting.Application.Common.Models.Result<bool>>, Accounting.Application.Features.Vouchers.Handlers.DeleteVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.PostVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.PostVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.ApproveVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.ApproveVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.RejectVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.RejectVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.SubmitVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.SubmitVoucherCommandHandler>();
            services.AddScoped<Accounting.Application.Common.Commands.ICommandHandler<Accounting.Application.Features.Vouchers.Commands.CancelVoucherCommand, Accounting.Application.Common.Models.Result<Accounting.Application.DTOs.VoucherDto>>, Accounting.Application.Features.Vouchers.Handlers.CancelVoucherCommandHandler>();

            // Register Banks Handlers
            services.AddScoped<Accounting.Application.Features.Banks.Handlers.CreateBankCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Banks.Handlers.UpdateBankCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Banks.Handlers.DeleteBankCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Banks.Handlers.GetBanksQueryHandler>();
            services.AddScoped<Accounting.Application.Features.Banks.Handlers.GetBankByIdQueryHandler>();

            // Register Locations Handlers
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.CreateLocationCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.UpdateLocationCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.DeleteLocationCommandHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.GetLocationsQueryHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.GetLocationByIdQueryHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.GetCountriesQueryHandler>();
            services.AddScoped<Accounting.Application.Features.Locations.Handlers.GetCitiesByCountryQueryHandler>();

            // Register Audit Logs Handlers
            services.AddScoped<Accounting.Application.Features.AuditLogs.Handlers.GetAuditLogsQueryHandler>();

            // Add MediatR
            services.AddMediatR(typeof(CreateTicketCommandHandler).Assembly);

            // Add AutoMapper
            services.AddAutoMapper(typeof(CreateTicketCommandHandler).Assembly);

            // Add Authorization
            services.AddAuthorization(options =>
            {
                foreach (var permission in typeof(Permissions).GetFields())
                {
                    if (permission.IsLiteral && !permission.IsInitOnly)
                    {
                        var permissionValue = permission.GetValue(null)?.ToString();
                        if (!string.IsNullOrEmpty(permissionValue))
                        {
                            options.AddPolicy(permissionValue, policy =>
                                policy.Requirements.Add(new PermissionRequirement(permissionValue)));
                        }
                    }
                }
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // Add JWT Authentication
            var jwtKey = Configuration["Jwt:Key"];
            if (!string.IsNullOrEmpty(jwtKey))
            {
                var key = Encoding.ASCII.GetBytes(jwtKey);
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            }
            else
            {
                // Add basic authentication if JWT key is not configured
                services.AddAuthentication();
            }

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounting.API", Version = "v1" });
                
                // Add JWT to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounting.API v1"));
            }

            // Initialize database
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                
                try
                {
                    // Wait for SQL Server to be ready
                    var retryCount = 0;
                    var maxRetries = 30;
                    
                    while (retryCount < maxRetries)
                    {
                        try
                        {
                            context.Database.Migrate();
                            logger.LogInformation("Database migration completed successfully.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            retryCount++;
                            logger.LogWarning($"Database migration attempt {retryCount} failed: {ex.Message}");
                            
                            if (retryCount >= maxRetries)
                            {
                                logger.LogError($"Failed to migrate database after {maxRetries} attempts: {ex}");
                                throw;
                            }
                            
                            Thread.Sleep(2000); // Wait 2 seconds before retry
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Database initialization failed: {ex}");
                    throw;
                }
            }

            // app.UseHttpsRedirection(); // Temporarily disabled for testing

            // Add CORS
            app.UseCors("AllowAll");

            // Add Audit Middleware
            app.UseMiddleware<AuditMiddleware>();

            // Configure static files for profile pictures
            var uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });

            app.UseRouting();

            // Add Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
