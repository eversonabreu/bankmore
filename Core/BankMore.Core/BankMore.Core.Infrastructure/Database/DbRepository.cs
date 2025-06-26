using BankMore.Core.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankMore.Core.Infrastructure.Database;

public class DbRepository<TEntity, TContext>(TContext context) 
    : IDbRepository<TEntity> where TEntity : Entity where TContext : DbContext
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity> GetByIdAsync(Guid id)
        => await SingleAsync(x => x.Id == id);

    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.SingleAsync(expression);

    public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.SingleOrDefaultAsync(expression);

    public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.FirstAsync(expression);

    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.FirstOrDefaultAsync(expression);

    public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.LastAsync(expression);

    public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.LastOrDefaultAsync(expression);

    public async Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.Where(expression).ToListAsync();

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        entity.Id = Guid.NewGuid();
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteByIdAsync(Guid id)
    {
        var entity = await SingleOrDefaultAsync(x => x.Id == id) 
            ?? throw new InvalidOperationException($"Entity with Id {id} not found.");
        _dbSet.Remove(entity);
    }

    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}