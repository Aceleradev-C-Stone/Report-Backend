using Report.Core.Dto.Responses;

namespace Report.IntegrationTests.Comparers
{
    public class UserResponseComparer : ResponseComparer
    {
        public override bool Equals(Response x, Response y)
        {
            var xData = x.Data as UserResponse;
            var yData = y.Data as UserResponse;

            if (xData.Id != yData.Id) return false;
            return base.Equals(x, y);
        }
    }
}