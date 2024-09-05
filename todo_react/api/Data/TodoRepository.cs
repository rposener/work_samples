using Azure;
using Azure.Data.Tables;
using System.Text.RegularExpressions;

namespace TodoApi.Data
{
    /// <summary>
    /// Todo Repository to Store <see cref="TodoEntity"/>
    /// </summary>
    public class TodoRepository : ITodoRepository, IRepositoryInitializer
    {
        private const string Todo_TableName = "todos";
        private static bool _created;
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<TodoRepository> _logger;
        private readonly string _storageSafeRegex = @"[A-Za-z0-9]+";

        public TodoRepository(TableServiceClient tableServiceClient, ILogger<TodoRepository> logger)
        {
            _created = false;
            _tableServiceClient = tableServiceClient;
            _logger = logger;
        }

        internal string GetSafeStorageKey(string todoList) =>
            string.Join("", Regex.Matches(todoList, _storageSafeRegex).Select(m => m.Value.ToLower()));

        /// <summary>
        /// Ensure that the Table has been Created
        /// </summary>
        public Task EnsureCreatedAsync() =>
            _created ? Task.CompletedTask :
            _tableServiceClient.CreateTableIfNotExistsAsync(Todo_TableName)
            .ContinueWith((_, _) => { _created = true; }, CancellationToken.None);

        /// <summary>
        /// Increment the Id of the Todo Id as a numeric string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal string IncrementId(string id)
        {
            int _id = 1;
            int.TryParse(id, out _id);
            _id = Math.Max(0, _id);
            _id++;
            return _id.ToString("00000000");
        }

        /// <summary>
        /// Calculates the NextId for a certain table partition
        /// </summary>
        /// <param name="todoPartition">Partition to be checked</param>
        /// <returns>Id that should be used</returns>
        /// <exception cref="Exception">Throws if Unable to Get the Next Id</exception>
        internal async Task<string> GetNextIdAsync(string todoPartition)
        {
            await EnsureCreatedAsync();

            string next_id = "0000000001";
            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            bool hasRetried = false;

        Retry_Get_Id:
            var response = await client.GetEntityIfExistsAsync<TodoIdentity>(todoPartition, "next_id");
            if (response.HasValue)
            {
                TodoIdentity identity = response.Value!;
                next_id = identity.NextId;
                identity.NextId = IncrementId(next_id);
                var update = await client.UpdateEntityAsync(identity, identity.ETag);
                if (update.IsError && !hasRetried)
                {
                    hasRetried = true;
                    goto Retry_Get_Id;
                    throw new Exception(update.ReasonPhrase);
                }
            }
            else
            {
                TodoIdentity identity = new TodoIdentity
                {
                    PartitionKey = todoPartition,
                    RowKey = "next_id",
                    NextId = IncrementId(next_id)
                };
                var update = await client.AddEntityAsync(identity);
                if (update.IsError && !hasRetried)
                {
                    hasRetried = true;
                    goto Retry_Get_Id;
                    throw new Exception(update.ReasonPhrase);
                }
            }
            return next_id;
        }

        /// <summary>
        /// Adds a Todo List Item to the Table
        /// </summary>
        /// <param name="todoList"></param>
        /// <param name="entity"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> AddTodoAsync(string todoList, TodoEntity entity)
        {
            await EnsureCreatedAsync();

            var todoPartition = GetSafeStorageKey(todoList);

            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            entity.PartitionKey = todoPartition;
            entity.RowKey = await GetNextIdAsync(todoList);
            var response = await client.AddEntityAsync(entity);
            if (!response.IsError && response.Headers.ETag.HasValue)
            {
                entity.ETag = response.Headers.ETag.Value;
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Updates a Todo List Item to the Table
        /// </summary>
        /// <param name="todoList"></param>
        /// <param name="entity"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> UpdateTodoAsync(string todoList, TodoEntity entity)
        {
            await EnsureCreatedAsync();

            var todoPartition = GetSafeStorageKey(todoList);

            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            entity.PartitionKey = todoPartition;
            entity.RowKey = entity.RowKey;
            var response = await client.UpsertEntityAsync(entity);
            if (!response.IsError && response.Headers.ETag.HasValue)
            {
                entity.ETag = response.Headers.ETag.Value;
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Removes a Todo List Item from the Table
        /// </summary>
        /// <param name="todoList"></param>
        /// <param name="rowKey">the RowKey</param>
        /// <returns>True if successful</returns>
        public async Task<bool> DeleteTodoAsync(string todoList, string rowKey)
        {
            await EnsureCreatedAsync();

            var todoPartition = GetSafeStorageKey(todoList);

            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            var response = await client.DeleteEntityAsync(todoPartition, rowKey);
            if (response.IsError)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns List of Todo Items stored in a todo List
        /// </summary>
        /// <param name="todoList"></param>
        /// <returns>List of Todo Items</returns>
        public async Task<IList<TodoEntity>> GetTodosAsync(string todoList)
        {
            await EnsureCreatedAsync();
            List<TodoEntity> result = new();

            var todoPartition = GetSafeStorageKey(todoList);
            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            var pages = client.QueryAsync<TodoEntity>($"PartitionKey eq '{todoPartition}'");
            await foreach (var todo in pages)
            {
                result.Add(todo);
            }

            _logger.LogInformation("Returned {todo_count} for {todoList} todo list.", result.Count, todoList);
            return result;
        }
    }
}
