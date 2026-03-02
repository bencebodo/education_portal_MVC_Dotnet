using EduPortal.Data.EPDbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EduPortal.Repo.BaseRepo
{
    public class EFBaseRepository<TEntity>(EduPortalDbContext _context) : IRepository<TEntity> where TEntity : class
    {

        public EduPortalDbContext GetDbContext() => _context;

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync<TKey>(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity?> AddAsync(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity?> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return await SaveAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TDerived>> GetAllByTypeAsync<TDerived>() where TDerived : TEntity
        {
            return await _context.Set<TEntity>()
                .OfType<TDerived>()
                .ToListAsync();
        }
    }
}
