using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Data.Context;
using Report.Domain.Models;

namespace Report.Data.Repositories
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