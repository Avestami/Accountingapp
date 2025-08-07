using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Accounting.Domain.Entities;

namespace Accounting.Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        // Query methods
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        
        // Query with includes
        Task<T> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        
        // Paging
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>> orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes);
        
        // CRUD operations
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<T> UpdateAsync(T entity);
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task SoftDeleteAsync(int id);
        Task SoftDeleteAsync(T entity);
        
        // Queryable for complex queries
        IQueryable<T> Query();
        IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes);
    }
}