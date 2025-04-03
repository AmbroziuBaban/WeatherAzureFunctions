using Azure;
using Azure.Data.Tables;

namespace GetLogsBetweenDates.Models
{
    public class DataProcessingLogEntry : ITableEntity
    {
        public string ProcessingMessageID { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
