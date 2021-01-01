using System;

namespace DevOidc.Repositories.Abstractions
{
    public interface ISpecification<TEntity, TModel> : ISelection<TEntity>
    {
        Func<TEntity, TModel> Projection { get; }
    }
}
