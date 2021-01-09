using System;

namespace DevOidc.Repositories.Abstractions
{

    public interface IOperation<TEntity> : ISelection<TEntity>
    {
        Action<TEntity> Mutation { get; }
    }
}
