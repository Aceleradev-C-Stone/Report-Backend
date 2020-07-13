using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Core.Models;
using Report.Core.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) {}

        public Task<User> GetByEmail(string email)
        {
            return _dbContext.Set<User>()
                             .AsNoTracking()
                             .FirstOrDefaultAsync(user => user.Email == email);
        }
    }
}