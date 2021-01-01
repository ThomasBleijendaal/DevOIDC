using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace DevOidc.Repositories.Abstractions
{
    public interface IReadRepository<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        Task<TModel?> GetAsync<TModel>(ISpecification<TEntity, TModel> specification);
        Task<IReadOnlyList<TModel>> GetListAsync<TModel>(ISpecification<TEntity, TModel> specification);
    }
}
