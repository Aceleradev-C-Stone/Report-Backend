using System;
using Report.Core.Dto.Responses;

namespace Report.IntegrationTests.Comparers
{
    public class UserResponseListComparer : ResponseComparer
    {
        public override bool Equals(Response x, Response y)
        {
            var xData = x.Data as UserResponse[];
            var yData = y.Data as UserResponse[];

            if (xData.Length != yData.Length) return false;

            for (var i = 0; i < xData.Length; i++)
            {
                if (xData[i].Id != yData[i].Id) return false;
            }

            return base.Equals(x, y);
        }
    }
}