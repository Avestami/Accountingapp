using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Accounting.Application.Common.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var company = context.User.FindFirst("company")?.Value;

            if (string.IsNullOrEmpty(userRole) || string.IsNullOrEmpty(company))
            {
                return Task.CompletedTask;
            }

            if (HasPermission(userRole, requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool HasPermission(string role, string permission)
        {
            return role.ToLower() switch
            {
                "admin" => true, // Admin has all permissions
                "finance" => IsFinancePermission(permission),
                "sales" => IsSalesPermission(permission),
                "viewer" => IsViewPermission(permission),
                _ => false
            };
        }

        private bool IsFinancePermission(string permission)
        {
            return permission.StartsWith("finance.") ||
                   permission.StartsWith("vouchers.") ||
                   permission.StartsWith("reports.") ||
                   permission == Permissions.TicketsView ||
                   permission == Permissions.SettingsView;
        }

        private bool IsSalesPermission(string permission)
        {
            return permission.StartsWith("tickets.") ||
                   permission == Permissions.ReportsView ||
                   permission == Permissions.SettingsView;
        }

        private bool IsViewPermission(string permission)
        {
            return permission.EndsWith(".view");
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}