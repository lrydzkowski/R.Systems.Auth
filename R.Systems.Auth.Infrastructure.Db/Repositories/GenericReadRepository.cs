using Microsoft.EntityFrameworkCore;
using R.Systems.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R.Systems.Auth.Infrastructure.Db.Repositories;

public class GenericReadRepository<T> : IGenericReadRepository<T> where T : class, IEntity, new()
{
    public GenericReadRepository(AuthDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public AuthDbContext DbContext { get; }

    protected Expression<Func<T, long?>> OnlyId { get; } = entity => entity.Id;

    protected virtual Expression<Func<T, T>> Entities { get; } = entity => new T();

    public async Task<T?> GetAsync(long recId)
    {
        T? entity = await GetQuery().Where(entity => entity.Id == recId).FirstOrDefaultAsync();
        return entity;
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> whereExpression)
    {
        T? entity = await GetQuery().Where(whereExpression).FirstOrDefaultAsync();
        return entity;
    }

    public async Task<List<T>> GetAsync()
    {
        List<T> entities = await GetQuery().ToListAsync();
        return entities;
    }

    private IQueryable<T> GetQuery()
    {
        return DbContext.Set<T>().AsNoTracking().Select(Entities);
    }
}
