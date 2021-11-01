using R.Systems.Auth.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<T?> GetAsync(long recId);

        public Task<T?> GetAsync(Expression<Func<User, bool>> whereExpression);

        public Task<List<T>> GetAsync();

        public Task<bool> EditAsync(T user);
    }
}
