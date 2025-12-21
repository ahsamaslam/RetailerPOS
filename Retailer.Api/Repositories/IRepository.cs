using Retailer.POS.Api.Entities;
using System.Linq.Expressions;
namespace Retailer.POS.Api.Repositories;

public interface IRepository<T> where T : class
{
    //Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity); 
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null);

    //IQueryable<T> Query();
}
