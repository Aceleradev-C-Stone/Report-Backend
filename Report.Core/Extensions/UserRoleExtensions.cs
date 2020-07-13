using System;
using Report.Core.Enums;

namespace Report.Core.Extensions
{
    public static class UserRoleExtensions
    {
        public static string GetName(this EUserRole role)
        {
            return Enum.GetName(typeof(EUserRole), role);
        }
    }
}