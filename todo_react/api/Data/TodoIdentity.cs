using Azure;
using Azure.Data.Tables;

namespace TodoApi.Data
{
    public class TodoIdentity : ITableEntity
    {
        /// <summary>
        /// Name of the Todo List
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Always has a value of next_id
        /// </summary>
        public string RowKey { get; set; } = "next_id";

        /// <summary>
        /// The Next Id to be Used for the Todo List
        /// </summary>
        public string NextId { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
