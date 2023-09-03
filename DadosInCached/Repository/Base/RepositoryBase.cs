using DadosInCached.Context;
using DadosInCached.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DadosInCached.Repository.Base
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, new()
    {
        protected readonly AppDbContext _context;
        protected DbSet<TEntity> DbSet { get; }

        protected RepositoryBase(IServiceProvider service)
        {
            _context = service.GetRequiredService<AppDbContext>();
            DbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression != null)
                return DbSet.Where(expression);

            return DbSet;
        }

        public async Task<TEntity> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public async Task InsertAsync(TEntity entity) => await DbSet.AddAsync(entity);

        public void InsertRange(List<TEntity> entity) => DbSet.AddRange(entity);

        public void UpdateAsync(TEntity entity) => DbSet.Update(entity);

        public void DeleteAsync(TEntity entity) => DbSet.Remove(entity);

        public void DeleteRangeAsync(List<TEntity> entityArray) => DbSet.RemoveRange(entityArray);

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
