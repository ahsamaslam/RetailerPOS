using Microsoft.EntityFrameworkCore;
using Retailer.POS.Api.Data;
using Retailer.POS.Api.Entities;
using System.Linq.Expressions;

namespace Retailer.POS.Api.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly RetailerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(RetailerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return query;
    }
}
