using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.queues
{
    public class AdminNotificationQueue
    {
        public static CloudQueue GetQueue(string queueName)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(queueName);
                queue.CreateIfNotExists();
                return queue;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            return null;
        }

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
