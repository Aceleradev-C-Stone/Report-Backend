using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Report.Core.Enums;
using Report.Infra.Extensions;

namespace Report.Api.Authorization
{
    public class AuthorizeUserRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeUserRolesAttribute(params EUserRole[] allowedRoles)
        {
            Roles = string.Join(",", GetRolesNames(allowedRoles));
        }

        private IEnumerable<string> GetRolesNames(EUserRole[] roles)
        {
            return roles.Select(role => role.GetName());
        }
    }
}