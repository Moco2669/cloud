using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.queues
{
    public class AdminNotificationQueue
    {
        public static void EnqueueMessage(CloudQueue queue, string message)
        {
            CloudQueueMessage queueMessage = new CloudQueueMessage(message);
            queue.AddMessageAsync(queueMessage);
        }

        public static string DequeueMessage(CloudQueue queue)
        {
            CloudQueueMessage queueMessage = queue.GetMessage();
            if(queueMessage != null)
            {
                return queueMessage.AsString;
            }
            return null;
        }
    }
}
