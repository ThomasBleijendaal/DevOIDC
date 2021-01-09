using System;

namespace DevOidc.Repositories.Abstractions
{
    public interface ICreation<TEntity>
    {
        string PartitionKey { get; }

        Action<TEntity> Mutation { get; }

        string CreatedId { set; }
    }
}
