using Microsoft.WindowsAzure.Storage.Queue;
using RedditDataRepository.classes.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.queues
{
    public class NotificationQueue
    {
        public static void EnqueueComment(CloudQueue queue, Comment comment)
        {
            CloudQueueMessage message = new CloudQueueMessage(comment.Id);
            queue.AddMessageAsync(message);
        }

        public static string DequeueComment(CloudQueue queue)
        {
            CloudQueueMessage message = queue.GetMessage();
            if (message != null)
            {
                queue.DeleteMessage(message);
                return message.AsString;
            }
            return null;
        }
    }
}
