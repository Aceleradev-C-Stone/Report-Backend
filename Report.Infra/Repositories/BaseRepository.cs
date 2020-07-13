using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Report.Core.Repositories;
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

        public virtual void Add(TEntity entity)
        {
            _dbContext.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbContext.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public virtual async Task<TEntity[]> GetAll()
        {
            return await _dbContext.Set<TEntity>()
                                   .AsNoTracking()
                                   .ToArrayAsync();
        }

        public virtual async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }
}