using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Votes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditDataRepository.votes.Read
{
    /// <summary>
    /// Provides functionality to retrieve all votes for a specified post from a cloud table.
    /// </summary>
    public class VotesCount
    {
        /// <summary>
        /// Retrieves all votes for the specified post from the cloud table.
        /// </summary>
        /// <param name="table">The cloud table containing the vote records.</param>
        /// <param name="postId">The ID of the post to retrieve votes for.</param>
        /// <returns>A list of votes for the specified post.</returns>
        public static async Task<List<Vote>> Execute(CloudTable table, string postId)
        {
            // Generate a table query to retrieve all votes for the specified post
            var query = new TableQuery<Vote>().Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Vote"),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("PostId", QueryComparisons.Equal, postId)));

            // Initialize a list to store the retrieved votes
            List<Vote> votes = new List<Vote>();

            // Execute the query asynchronously and retrieve the results
            var queryResult = await table.ExecuteQuerySegmentedAsync(query, null);

            // Add the retrieved votes to the list
            votes.AddRange(queryResult.Results);

            // Return the list of votes
            return votes;
        }
    }
}
