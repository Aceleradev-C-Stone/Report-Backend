using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Models;
using Report.Domain.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) {}

        Task<User> IUserRepository.GetByEmail(string email)
        {
            return _dbContext.Set<User>()
                        .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}