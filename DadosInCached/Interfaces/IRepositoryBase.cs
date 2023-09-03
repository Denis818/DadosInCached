using DadosInCached.Models;
using System.Linq.Expressions;

namespace DadosInCached.Interfaces
{
    public interface IRepositoryBase<TEntity> where TEntity : class, new()
    {
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null);
        Task<TEntity> GetByIdAsync(int id);
        Task InsertAsync(TEntity entity);
        void InsertRange(List<TEntity> entity);
        void UpdateAsync(TEntity entity);
        void DeleteAsync(TEntity entity);
        void DeleteRangeAsync(List<TEntity> entity);
        Task<bool> SaveChangesAsync();
    }
}
