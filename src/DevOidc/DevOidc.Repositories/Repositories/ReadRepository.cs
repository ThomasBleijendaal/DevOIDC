using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using DevOidc.Repositories.Abstractions;

namespace DevOidc.Repositories.Repositories
{
    public class ReadRepository<TEntity> : IReadRepository<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        private readonly TableServiceClient _client;

        public ReadRepository(TableServiceClient client)
        {
            _client = client;
        }

        public async Task<TModel?> GetAsync<TModel>(ISpecification<TEntity, TModel> specification)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            await foreach (var element in table.QueryAsync(specification.Criteria).ConfigureAwait(false))
            {
                return specification.Projection.Invoke(element);
            }

            return default;
        }

        public async Task<IReadOnlyList<TModel>> GetListAsync<TModel>(ISpecification<TEntity, TModel> specification)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            var result = new List<TModel>();

            await foreach (var element in table.QueryAsync(specification.Criteria).ConfigureAwait(false))
            {
                result.Add(specification.Projection.Invoke(element));
            }

            return result;
        }

        private async Task<TableClient> GetTableAsync()
        {
            var table = _client.GetTableClient(typeof(TEntity).Name.ToLowerInvariant());

            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

            return table;
        }
    }
}
