using System.Collections.Generic;
using Report.Core.Dto.Responses;

namespace Report.Tests.Comparers
{
    public class ResponseComparer : IEqualityComparer<Response>
    {
        public virtual bool Equals(Response x, Response y)
        {
            return x.Code == y.Code && x.Message == y.Message;
        }

        public int GetHashCode(Response obj)
        {
            if (obj.Data != null)
                return obj.Data.GetHashCode();
            else if (obj.Message != null)
                return obj.Message.GetHashCode();
            else
                return obj.Code.GetHashCode();
        }
    }
}