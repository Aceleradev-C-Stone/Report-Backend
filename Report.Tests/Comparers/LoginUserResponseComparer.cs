using Report.Core.Dto.Responses;

namespace Report.Tests.Comparers
{
    public class LoginUserResponseComparer : ResponseComparer
    {
        public override bool Equals(Response x, Response y)
        {
            var xData = x.Data as LoginUserResponse;
            var yData = y.Data as LoginUserResponse;

            if (xData.Token != yData.Token) return false;
            if (xData.User.Id != yData.User.Id) return false;
            if (xData.ExpiresIn != yData.ExpiresIn) return false;
            
            return base.Equals(x, y);
        }
    }
}