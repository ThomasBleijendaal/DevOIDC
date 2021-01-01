using System.Threading.Tasks;
using Azure.Data.Tables;

namespace DevOidc.Repositories.Abstractions
{
    public interface IWriteRepository<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        Task CreateEntityAsync(ICreation<TEntity> creation);
        Task UpdateSingleEntityAsync(IOperation<TEntity> operation);
        Task UpdateMultipleEntitiesAsync(IOperation<TEntity> operation);
        Task DeleteEntitiesAsync(ISelection<TEntity> selection);
    }
}
