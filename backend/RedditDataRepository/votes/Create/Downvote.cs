using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Votes;
using RedditDataRepository.votes.Read;
using System.Threading.Tasks;

namespace RedditDataRepository.votes.Create
{
    /// <summary>
    /// Provides functionality to handle downvoting operations on posts.
    /// </summary>
    public class Downvote
    {
        /// <summary>
        /// Executes a downvoting operation on a post.
        /// </summary>
        /// <param name="table">The cloud table containing vote records.</param>
        /// <param name="vote">The vote object representing the downvote.</param>
        /// <returns>True if the downvoting operation was successful; otherwise, false.</returns>
        public static async Task<bool> Execute(CloudTable table, Vote vote)
        {
            // Check if vote or table is null
            if (vote == null || table == null)
                return false;

            try
            {
                // Check if the vote exists in the table
                Vote v = await VoteExists.DoesVoteExist(table, vote.PostId, vote.Email);

                if (v == null)
                {
                    // Vote does not exist, insert the downvote
                    // Create a TableOperation object to insert or replace the entity
                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(vote);

                    // Execute the operation asynchronously
                    TableResult result = await table.ExecuteAsync(insertOrReplaceOperation);

                    // Check if the operation was successful
                    return result.HttpStatusCode == 200 || result.HttpStatusCode == 204;
                }
                else if (v.Voted)
                {
                    // Vote exists and is an upvote, replace with downvote
                    TableOperation delete = TableOperation.Delete(v);
                    TableResult deleteResult = await table.ExecuteAsync(delete);

                    // Create a TableOperation object to insert or replace the entity
                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(vote);

                    // Execute the operation asynchronously
                    TableResult result = await table.ExecuteAsync(insertOrReplaceOperation);

                    // Check if the operation was successful
                    return result.HttpStatusCode == 200 || result.HttpStatusCode == 204;
                }
                else
                {
                    // Vote exists and is already a downvote, remove it
                    TableOperation delete = TableOperation.Delete(v);

                    TableResult result = await table.ExecuteAsync(delete);

                    return result.HttpStatusCode == 200 || result.HttpStatusCode == 204;
                }
            }
            catch
            {
                // An error occurred during insertion
                return false;
            }
        }
    }
}
