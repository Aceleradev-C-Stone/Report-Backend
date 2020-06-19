using System.Threading.Tasks;
using Report.Domain.Models;

namespace Report.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email);
    }
}