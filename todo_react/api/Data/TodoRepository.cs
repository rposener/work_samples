using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Http.Logging;
using Namotion.Reflection;
using System.Text.RegularExpressions;

namespace TodoApi.Data
{
    /// <summary>
    /// Todo Repository to Store <see cref="TodoEntity"/>
    /// </summary>
    public class TodoRepository : ITodoRepository, IRepositoryInitializer
    {
        const string Todo_TableName = "todos";
        private static bool _created = false;
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<TodoRepository> _logger;
        private readonly IIdentityManager _identityManager;
        private readonly string _storageSafeRegex = @"[A-Za-z0-9]+";

        public TodoRepository(TableServiceClient tableServiceClient, ILogger<TodoRepository> logger, IIdentityManager identityManager)
        {
            _tableServiceClient = tableServiceClient;
            _logger = logger;
            _identityManager = identityManager;
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
        /// Adds a Todo List Item to the Table
        /// </summary>
        /// <param name="todoList"></param>
        /// <param name="entity"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> AddTodoAsync(string todoList, TodoEntity entity)
        {
            var todoPartition = GetSafeStorageKey(todoList);

            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            entity.PartitionKey = todoPartition;
            entity.RowKey = await _identityManager.GetNextIdAsync(todoList);
            var response = await client.AddEntityAsync(entity);
            if (response.IsError)
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

            var todoPartition = GetSafeStorageKey(todoList);

            var client = _tableServiceClient.GetTableClient(Todo_TableName);
            entity.PartitionKey = todoPartition;
            entity.RowKey = entity.RowKey;
            var response = await client.UpsertEntityAsync(entity);
            if (response.IsError)
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
