using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.posts.Update
{
    /// <summary>
    /// Provides methods to insert a subscription entity into an Azure Storage Table.
    /// </summary>
    public class SubscribeToPost
    {
        /// <summary>
        /// Inserts a subscription entity into the specified Azure Storage Table asynchronously.
        /// </summary>
        /// <param name="table">The Azure Storage Table to insert the subscription to.</param>
        /// <param name="postId">ID of the post to subscribe to</param>
        /// <returns>
        /// A task representing the asynchronous operation. Returns <c>true</c> if the insertion was successful; otherwise, <c>false</c>.
        /// </returns>
        public static async Task<bool> Execute(CloudTable table, string postId, string email)
        {
            if (table == null)
            {
                return false;
            }
            try
            {
                TableOperation tableOperation = TableOperation.InsertOrReplace(new Subscription(postId, email));
                TableResult tableResult = await table.ExecuteAsync(tableOperation);

                return tableResult.HttpStatusCode == 200 || tableResult.HttpStatusCode == 204;
            }
            catch
            {
                return false;
            }
        }
    }
}
