using System.Threading.Tasks;
using Report.Core.Models;

namespace Report.Core.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email);
    }
}