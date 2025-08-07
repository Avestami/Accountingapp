using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Accounting.Domain.Entities;
using Accounting.Infrastructure.Data;
using Accounting.Application.Interfaces;

namespace Accounting.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AccountingDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AccountingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).FirstOrDefaultAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).AnyAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _dbSet.Where(e => !e.IsDeleted).CountAsync();
            
            return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).CountAsync();
        }

        public virtual async Task<T> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query.Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return await query.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, object>> orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            // Apply includes
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            // Apply filter
            query = query.Where(e => !e.IsDeleted);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            
            // Get total count before paging
            var totalCount = await query.CountAsync();
            
            // Apply ordering
            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }
            
            // Apply paging
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (items, totalCount);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
            var entityList = entities.ToList();
            var now = DateTime.UtcNow;
            
            foreach (var entity in entityList)
            {
                entity.CreatedAt = now;
                entity.UpdatedAt = now;
            }
            
            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            
            return await Task.FromResult(entity);
        }

        public virtual async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
            var entityList = entities.ToList();
            var now = DateTime.UtcNow;
            
            foreach (var entity in entityList)
            {
                entity.UpdatedAt = now;
            }
            
            _dbSet.UpdateRange(entityList);
            return await Task.FromResult(entityList);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

        public virtual async Task SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(entity);
            }
        }

        public virtual async Task SoftDeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
        }

        public virtual IQueryable<T> Query()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        public virtual IQueryable<T> QueryWithIncludes(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            
            return query.Where(e => !e.IsDeleted);
        }
    }
}