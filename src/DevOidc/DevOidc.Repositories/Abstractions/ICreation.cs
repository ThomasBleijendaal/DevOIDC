using System;

namespace DevOidc.Repositories.Abstractions
{
    public interface ICreation<TEntity>
    {
        string OwnerId { get; }

        Action<TEntity> Mutation { get; }

        string CreatedId { set; }
    }
}
