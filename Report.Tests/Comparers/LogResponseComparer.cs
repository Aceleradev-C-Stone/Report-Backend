using Report.Core.Dto.Responses;

namespace Report.Tests.Comparers
{
    public class LogResponseComparer : ResponseComparer
    {
        public override bool Equals(Response x, Response y)
        {
            var xData = x.Data as LogResponse;
            var yData = y.Data as LogResponse;

            if (xData.Id != yData.Id) return false;
            return base.Equals(x, y);
        }
    }
}