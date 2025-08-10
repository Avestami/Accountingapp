using Microsoft.AspNetCore.Authorization;
using System;

namespace Accounting.Application.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public Permission Permission { get; }

        public RequirePermissionAttribute(Permission permission)
        {
            Permission = permission;
            Policy = permission.ToString();
        }
    }
}