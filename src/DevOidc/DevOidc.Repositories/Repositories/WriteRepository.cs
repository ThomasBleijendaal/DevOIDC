﻿using System;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using DevOidc.Repositories.Abstractions;
using DevOidc.Repositories.Exceptions;

namespace DevOidc.Repositories.Repositories
{
    public class WriteRepository<TEntity> : IWriteRepository<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        private readonly TableServiceClient _client;

        public WriteRepository(TableServiceClient client)
        {
            _client = client;
        }

        public async Task CreateEntityAsync(ICreation<TEntity> creation)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            var entity = new TEntity
            {
                PartitionKey = creation.PartitionKey.ToString()
            };

            creation.Mutation.Invoke(entity);

            if (string.IsNullOrEmpty(entity.RowKey))
            {
                entity.RowKey = Guid.NewGuid().ToString();
            }

            await table.AddEntityAsync(entity).ConfigureAwait(false);

            creation.CreatedId = entity.RowKey;
        }

        public async Task ReinsertEntityAsync(IOperation<TEntity> operation)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            await foreach (var entity in table.QueryAsync(operation.Criteria).ConfigureAwait(false))
            {
                await ReinsertEntityAsync(operation, table, entity).ConfigureAwait(false);
                break;
            }
        }

        public async Task DeleteEntitiesAsync(ISelection<TEntity> selection)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            await foreach (var entity in table.QueryAsync(selection.Criteria).ConfigureAwait(false))
            {
                await table.DeleteEntityAsync(entity.PartitionKey, entity.RowKey).ConfigureAwait(false);
            }
        }

        public async Task UpdateSingleEntityAsync(IOperation<TEntity> operation)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            await foreach (var entity in table.QueryAsync(operation.Criteria).ConfigureAwait(false))
            {
                await UpdateEntityAsync(operation, table, entity).ConfigureAwait(false);
                break;
            }
        }

        public async Task UpdateMultipleEntitiesAsync(IOperation<TEntity> operation)
        {
            var table = await GetTableAsync().ConfigureAwait(false);

            await foreach (var entity in table.QueryAsync(operation.Criteria).ConfigureAwait(false))
            {
                await UpdateEntityAsync(operation, table, entity).ConfigureAwait(false);
            }
        }

        private static async Task UpdateEntityAsync(IOperation<TEntity> operation, TableClient table, TEntity entity)
        {
            operation.Mutation.Invoke(entity);

            try
            {
                await table.UpdateEntityAsync(entity, entity.ETag, TableUpdateMode.Replace).ConfigureAwait(false);
            }
            catch (RequestFailedException)
            {
                throw new ConflictException();
            }
        }

        private static async Task ReinsertEntityAsync(IOperation<TEntity> operation, TableClient table, TEntity entity)
        {
            await table.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag);

            operation.Mutation.Invoke(entity);

            try
            {
                await table.AddEntityAsync(entity).ConfigureAwait(false);
            }
            catch (RequestFailedException)
            {
                throw new ConflictException();
            }
        }

        private async Task<TableClient> GetTableAsync()
        {
            var table = _client.GetTableClient(typeof(TEntity).Name.ToLowerInvariant());

            await table.CreateIfNotExistsAsync().ConfigureAwait(false);

            return table;
        }
    }
}
