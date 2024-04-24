using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.classes.Posts
{
    public class Subscription : TableEntity
    {
        public string PostId { get; set; }
        public string Email { get; set; }

        public Subscription() { }
        public Subscription(string postId, string email)
        {
            PartitionKey = "Subscription";
            RowKey = postId + email;
            PostId = postId;
            Email = email;
        }
    }
}
