using Common.auth.guard;
using Common.cloud.account;
using Common.cloud.queue;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditDataRepository.classes.Comments;
using RedditDataRepository.comments.Create;
using RedditDataRepository.comments.Delete;
using RedditDataRepository.comments.Read;
using RedditDataRepository.Comments.Read;
using RedditDataRepository.queues;
using RedditServiceWorker.Models.comment;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace RedditServiceWorker.Controllers
{
    [RoutePrefix("api/comment")]
    public class CommentController : ApiController
    {
        #region CREATE
        /// <summary>
        /// Controller method for creating a new comment.
        /// </summary>
        /// <param name="_comment">The comment data to be created.</param>
        /// <returns>An <see cref="IHttpActionResult"/> representing the result of the operation.</returns>
        [Route("create")]
        [HttpPost]
        [JwtAuthenticationFilter] // Requires JWT authentication
        public async Task<IHttpActionResult> Create(CreateComment _comment)
        {
            try
            {
                // Create a new Comment object
                Comment comment = new Comment(_comment.Author, _comment.PostId, _comment.Content);

                // Insert the comment into the Azure table
                bool insert_result = await InsertComment.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), comment);

                if (insert_result)
                {
                    // Comment was successfully added to the table
                    // Insert new comment into Queue for NotificationService to process
                    CloudQueue queue = AzureQueueHelper.GetQueue("notifications");
                    NotificationQueue.EnqueueComment(queue, comment);
                    return Ok(); // Return 200 OK
                }
                else
                {
                    // Return 400 Bad Request with an error message
                    return BadRequest("Comment creation failed");
                }
            }
            catch (Exception e)
            {
                // Return 500 Internal Server Error with the exception details
                return InternalServerError(e);
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Controller method for deleting a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to be deleted.</param>
        /// <returns>An <see cref="IHttpActionResult"/> representing the result of the operation.</returns>
        [HttpDelete]
        [Route("{commentId}")]
        [JwtAuthenticationFilter] // Requires JWT authentication
        public async Task<IHttpActionResult> Delete(string commentId)
        {
            try
            {
                // Retrieve the author of the comment by its ID
                string author = await ReadCommentAuthor.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), commentId);

                // Check if the current user is authorized to delete the comment
                if (!ResourceGuard.RunCheck(ActionContext, author))
                {
                    // Return 401 Unauthorized if the request is not authorized
                    return Unauthorized();
                }

                // Delete the comment from the Azure table
                bool delete_result = await DeleteComment.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), commentId);

                if (delete_result)
                {
                    // Return 200 OK if the comment is successfully deleted
                    return Ok();
                }
                else
                {
                    // Return 400 Bad Request if the deletion fails
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                // Return 500 Internal Server Error with the exception details
                return InternalServerError(e);
            }
        }
        #endregion

        #region COUNT

        /// <summary>
        /// Retrieves the count of comments on the specified post.
        /// </summary>
        /// <param name="postId">The ID of the post for which to count comments.</param>
        /// <returns>
        /// An IHttpActionResult containing the count of comments on the post if successful;
        /// otherwise, an InternalServerError result containing the encountered exception.
        /// </returns>
        [HttpGet]
        [Route("count/{postId}")]
        public async Task<IHttpActionResult> CountCommentsOnPost(string postId)
        {
            try
            {
                // Execute the CountComments.Execute method to retrieve the count of comments for the specified post
                var result = await CountComments.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), postId);

                // Return an OK result containing the count of comments on the post
                return Ok(result.Count);
            }
            catch (Exception e)
            {
                // Return an InternalServerError result containing the encountered exception if an error occurs
                return InternalServerError(e);
            }
        }

        #endregion
    }
}