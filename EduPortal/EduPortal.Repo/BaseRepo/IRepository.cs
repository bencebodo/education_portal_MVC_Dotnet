using EduPortal.Data.EPDbContext;
using System.Linq.Expressions;

namespace EduPortal.Repo.BaseRepo
{
    public interface IRepository<TEntity> where TEntity : class
    {
        EduPortalDbContext GetDbContext();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync<TKey>(TKey id);
        Task<TEntity?> AddAsync(TEntity entity);
        Task<TEntity?> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TDerived>> GetAllByTypeAsync<TDerived>() where TDerived : TEntity;
        Task<bool> SaveAsync();
    }
}
