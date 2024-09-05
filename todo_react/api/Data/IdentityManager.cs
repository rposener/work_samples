using Azure.Data.Tables;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TodoApi.Data
{
    /// <summary>
    /// Adapater Pattern to Create/Manage the issuance of Incrementing Id's
    /// </summary>
    public class IdentityManager : IRepositoryInitializer, IIdentityManager
    {
        const int NextId_MaxRetries = 2;
        const string Idenity_TableName = "identity";
        private readonly TableServiceClient _tableServiceClient;
        private readonly ILogger<IdentityManager> _logger;

        private static bool _created = false;


        public IdentityManager(TableServiceClient tableServiceClient, ILogger<IdentityManager> logger)
        {
            _tableServiceClient = tableServiceClient;
            _logger = logger;
        }

        /// <summary>
        /// Ensure that the Table has been Created
        /// </summary>
        public Task EnsureCreatedAsync() =>
            _created ? Task.CompletedTask :
            _tableServiceClient.CreateTableIfNotExistsAsync(Idenity_TableName)
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
            var newId = _id.ToString("00000000");
            _logger.LogInformation("Incremented to {new_id} from {old_id}", newId, id);
            return newId;
        }

        /// <summary>
        /// Calculates the NextId for a certain table partition
        /// </summary>
        /// <param name="todoPartition">Partition to be checked</param>
        /// <returns>Id that should be used</returns>
        /// <exception cref="Exception">Throws if Unable to Get the Next Id</exception>
        public async Task<string> GetNextIdAsync(string todoPartition)
        {
            var client = _tableServiceClient.GetTableClient(Idenity_TableName);
            string? next_id = null;
            int tryCount = 1;
            do
            {
                next_id = await GetNextIdInternalAsync(client, todoPartition);
                tryCount++;
            } while (next_id == null && tryCount <= NextId_MaxRetries);
            if (next_id == null)
            {
                throw new Exception("Unable to get next Id for the todoPartition");
            }
            _logger.LogInformation("Returning {new_id} for partition {todo_partition}", next_id, todoPartition);
            return next_id;
        }

        /// <summary>
        /// Internal Method to Try to get the Next Id for a partition
        /// </summary>
        /// <param name="client"></param>
        /// <param name="todoPartition"></param>
        /// <returns></returns>
        private async Task<string?> GetNextIdInternalAsync(TableClient client, string todoPartition)
        {
            string next_id = "00000001";
            var response = await client.GetEntityIfExistsAsync<TodoIdentity>(todoPartition, "next_id");
            if (response.HasValue)
            {
                TodoIdentity identity = response.Value!;
                next_id = IncrementId(identity.NextId);
                identity.NextId = next_id;
                var update = await client.UpdateEntityAsync(identity, identity.ETag);
                if (update.IsError)
                {
                    return null;
                }
            }
            else
            {
                TodoIdentity identity = new TodoIdentity
                {
                    PartitionKey = todoPartition,
                    RowKey = "next_id",
                    NextId = next_id
                };
                var update = await client.AddEntityAsync(identity);
                if (update.IsError)
                {
                    return null;
                }
            }
            return next_id;
        }
    }
}
