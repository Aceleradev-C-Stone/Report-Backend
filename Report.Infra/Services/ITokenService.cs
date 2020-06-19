using Report.Domain.Models;

namespace Report.Infra.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}