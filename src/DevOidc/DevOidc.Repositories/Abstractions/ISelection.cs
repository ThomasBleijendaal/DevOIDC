using System;
using System.Linq.Expressions;

namespace DevOidc.Repositories.Abstractions
{
    public interface ISelection<TEntity>
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
