using System;
using Report.Domain.Enums;

namespace Report.Infra.Extensions
{
    public static class UserRoleExtensions
    {
        public static string GetName(this EUserRole role)
        {
            return Enum.GetName(typeof(EUserRole), role);
        }
    }
}