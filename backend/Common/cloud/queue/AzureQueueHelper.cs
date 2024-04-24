using Common.cloud.account;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.cloud.queue
{
    public class AzureQueueHelper
    {
        public static CloudQueue GetQueue(string queueName)
        {
            CloudQueueClient client = AzureTableStorageCloudAccount.GetAccount().CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
        }
    }
}
