using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.classes.Logs
{
    public class EmailLog : TableEntity
    {
        public DateTime TimeOfEmails { get; set; }
        public string CommentId { get; set; }
        public int NumOfEmails { get; set; }
        public EmailLog(DateTime timeOfEmails, string commentId, int numOfEmails)
        {
            PartitionKey = "EmailLog";
            RowKey = Guid.NewGuid().ToString();
            TimeOfEmails = timeOfEmails;
            CommentId = commentId;
            NumOfEmails = numOfEmails;
        }
    }
}
