using STREAMIT.Core.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace STREAMIT.DataAccess.Repositories.Abstractions.Generic
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> GetAll(bool ignoreQueryFilter=false);
        Task<T?> GetByIdAsync(int id);
        Task<int> SaveChangesAsync();
        Task<T?> GetAsync(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
}
