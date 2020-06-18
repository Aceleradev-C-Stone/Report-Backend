using System.Threading.Tasks;
using Report.Domain.Models;

namespace Report.Data.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email);
    }
}