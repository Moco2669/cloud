using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System.Threading.Tasks;

namespace RedditDataRepository.posts.Read
{
    /// <summary>
    /// Provides methods to read posts from an Azure Storage Table.
    /// </summary>
    public class ReadPost
    {
        /// <summary>
        /// Retrieves a post from the specified Azure Storage Table by its partition key and ID.
        /// </summary>
        /// <param name="table">The CloudTable object representing the Azure Storage Table.</param>
        /// <param name="partitionKey">The partition key identifying the post.</param>
        /// <param name="postId">The ID of the post to retrieve.</param>
        /// <returns>
        /// A Task that, when completed, returns the retrieved Post object.
        /// If the post is not found or an error occurs, the method returns null.
        /// </returns>
        public static async Task<Post> Run(CloudTable table, string partitionKey, string postId)
        {
            try
            {
                // Check if the table is null
                if (table == null)
                    return null;

                // Create a query to retrieve the post based on partition key and ID
                var query = new TableQuery<Post>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, postId)));

                TableContinuationToken continuationToken = null;

                do
                {
                    // Execute the query segment asynchronously
                    var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = segment.ContinuationToken;

                    // Iterate through the results and return the post if found
                    foreach (var entity in segment.Results)
                    {
                        if (entity.Id == postId)
                            return entity;
                    }
                } while (continuationToken != null);

                // Return null if no matching post is found
                return null;
            }
            catch
            {
                // Return null if an error occurs
                return null;
            }
        }
    }
}