using Microsoft.EntityFrameworkCore;
using STREAMIT.Core.Entities.Common;
using STREAMIT.DataAccess.Contexts;
using STREAMIT.DataAccess.Repositories.Abstractions.Generic;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace STREAMIT.DataAccess.Repositories.Implementations.Generic
{

    internal class Repository<T>(AppDbContext _context) : IRepository<T> where T : BaseEntity
    {
        
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }


        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public IQueryable<T> GetAll(bool ignoreQueryFilter = false)
        {

            var query=  _context.Set<T>().AsQueryable();
            if (ignoreQueryFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            return query;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return  entity;
        }


        public async Task<int> SaveChangesAsync()
        {
          return  await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public Task<T?> GetAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            var entity= await _context.Set<T>().AnyAsync(expression);
            return entity;
        }


    }
}
