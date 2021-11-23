using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.SharedKernel.Interfaces
{
    public interface IGenericReadRepository<T> where T : class, IEntity, new()
    {
        public Task<T?> GetAsync(long recId);

        public Task<T?> GetAsync(Expression<Func<T, bool>> whereExpression);

        public Task<List<T>> GetAsync();
    }
}
