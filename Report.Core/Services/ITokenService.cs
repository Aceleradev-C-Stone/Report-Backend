using Report.Core.Models;

namespace Report.Core.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
        public int GetExpirationInSeconds();
        public int GetExpirationInMinutes();
        public int GetExpirationInHours();
    }
}