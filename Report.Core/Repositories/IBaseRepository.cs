using System.Collections.Generic;
using System.Threading.Tasks;

namespace Report.Core.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity[]> GetAll();
        Task<TEntity> GetById(int id);
        Task<bool> SaveChangesAsync();
    }
}