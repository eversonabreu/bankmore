using BankMore.Core.Infrastructure.Entities;
using System.Linq.Expressions;

namespace BankMore.Core.Infrastructure.Database;

public interface IDbRepository<TEntity> where TEntity : Entity
{
    public Task<TEntity> GetByIdAsync(Guid id);

    public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression);

    public Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression);

    public Task<TEntity> CreateAsync(TEntity entity);

    public Task UpdateAsync(TEntity entity);

    public Task DeleteByIdAsync(Guid id);

    public Task SaveChangesAsync();
}