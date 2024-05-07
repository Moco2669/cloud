using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Votes;
using System.Linq;
using System.Threading.Tasks;

namespace RedditDataRepository.votes.Read
{
    /// <summary>
    /// Provides functionality to check if a vote exists for a given post and user.
    /// </summary>
    public class VoteExists
    {
        /// <summary>
        /// Checks if a vote exists for the specified post and user in the cloud table.
        /// </summary>
        /// <param name="table">The cloud table containing the vote records.</param>
        /// <param name="postId">The ID of the post to check for.</param>
        /// <param name="email">The email address of the user to check for.</param>
        /// <returns>The vote object if it exists; otherwise, null.</returns>
        public static async Task<Vote> DoesVoteExist(CloudTable table, string postId, string email)
        {
            // Generate filter conditions for the partition key (Vote), post ID, and user ID
            string partitionFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Vote");
            string postIdFilter = TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, postId);
            string userIdFilter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email);

            // Combine the filters using AND operators
            string combinedFilter = TableQuery.CombineFilters(partitionFilter, TableOperators.And, postIdFilter);
            combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.And, userIdFilter);

            // Create a table query with the combined filter
            var query = new TableQuery<Vote>().Where(combinedFilter);

            // Execute the query asynchronously and retrieve the results
            var result = await table.ExecuteQuerySegmentedAsync(query, null);

            // Return the first result if any, otherwise return null
            return result.FirstOrDefault();
        }
    }
}
