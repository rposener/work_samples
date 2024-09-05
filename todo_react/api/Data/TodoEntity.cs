using Azure;
using Azure.Data.Tables;

namespace TodoApi.Data
{
    /// <summary>
    /// Todo Entity stored in a Storage Table
    /// </summary>
    public class TodoEntity : ITableEntity
    {
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public int? ReminderDays { get; set; }

        public bool IsCompleted { get; set; }

        /// <summary>
        /// Name of the TodoList
        /// </summary>
        public string PartitionKey { get; set; }
        
        /// <summary>
        /// Incrementing Id
        /// </summary>
        public string RowKey { get; set; }
        
        public DateTimeOffset? Timestamp { get; set; }
        
        public ETag ETag { get; set; }
    }
}
