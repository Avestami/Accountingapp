using Microsoft.AspNetCore.Authorization;
using System;

namespace Accounting.Application.Common.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission) : base(permission)
        {
            Policy = permission;
        }
    }
}