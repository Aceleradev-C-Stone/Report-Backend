using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Domain.Repositories;
using Report.Infra.Contexts;

namespace Report.Infra.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DataContext _dbContext;
        
        public BaseRepository(DataContext context)
        {
            _dbContext = context;
        }

        public void Add(TEntity entity)
        {
            _dbContext.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public async Task<TEntity[]> GetAll()
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .ToArrayAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }
}