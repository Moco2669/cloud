using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RedditDataRepository.posts.Read
{
    public class ReadPosts
    {
        public static async Task<List<Post>> Execute(CloudTable table, string searchKeywords, int pageSize, int pageNumber)
        {
            var query = new TableQuery<Post>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Post"));

            // Add search filter if search keywords are provided
            if (!string.IsNullOrEmpty(searchKeywords))
            {
                string[] searchTerms = searchKeywords.ToLower().Split(' '); // Assuming keywords are separated by spaces
                foreach (string term in searchTerms)
                {
                    // Use Equal with a wildcard (*) to perform a partial match
                    query = query.Where(TableQuery.GenerateFilterCondition("Title", QueryComparisons.Equal, $"*{term}*"));
                }
            }

            var posts = new List<Post>();
            TableContinuationToken continuationToken = null;

            // Implement pagination
            int itemsToSkip = pageSize * (pageNumber - 1);
            int itemsRetrieved = 0;

            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                posts.AddRange(queryResult.Results);
                continuationToken = queryResult.ContinuationToken;

                // Calculate the number of items retrieved so far
                itemsRetrieved += queryResult.Results.Count;

                // If we have retrieved enough items, break the loop
                if (itemsRetrieved >= itemsToSkip + pageSize)
                    break;

            } while (continuationToken != null);

            // Apply pagination
            var pagedPosts = posts.OrderByDescending(post => post.Timestamp)
                                  .Skip(itemsToSkip)
                                  .Take(pageSize)
                                  .ToList();

            return pagedPosts;
        }
    }
}
