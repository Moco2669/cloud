using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.posts.Read
{
    public class ReadSubscriptions
    {
        public static async Task<List<string>> Run(CloudTable table, string postId)
        {
            List<string> emails = new List<string>();
            var tableQuery = new TableQuery<Subscription>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Subscription"), TableOperators.And, TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, postId)));
            var result = table.ExecuteQuery(tableQuery);

            List<Subscription> subs = result.ToList();
            foreach(Subscription sub in subs)
            {
                emails.Add(sub.Email);
            }
            return emails;
        }
    }
}
