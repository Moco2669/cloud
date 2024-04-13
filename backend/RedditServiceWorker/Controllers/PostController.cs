using Common.auth.guard;
using Common.cloud.account;
using RedditDataRepository.blobs.images;
using RedditDataRepository.classes.Comments;
using RedditDataRepository.classes.Posts;
using RedditDataRepository.comments.Delete;
using RedditDataRepository.comments.Read;
using RedditDataRepository.posts.Create;
using RedditDataRepository.posts.Delete;
using RedditDataRepository.posts.Read;
using RedditDataRepository.Posts.Read;
using RedditServiceWorker.Models.post;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RedditServiceWorker.Controllers
{
    /// <summary>
    /// Controller for handling post-related operations.
    /// </summary>
    [RoutePrefix("api/post")]
    public class PostController : ApiController
    {
        #region CREATE
        /// <summary>
        /// Creates a new post.
        /// </summary>
        /// <returns>
        /// An <see cref="IHttpActionResult"/> representing the result of the operation.
        /// </returns>
        [Route("create")]
        [HttpPost]
        [JwtAuthenticationFilter] // Requires JWT authentication
        public async Task<IHttpActionResult> Create()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            // Store form data locally
            var provider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath("~/App_Data"));

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);

                // Access form data and put into post object
                Post post = new Post(provider);

                var has_image = bool.Parse(provider.FormData["hasImage"]);

                if (has_image)
                {
                    // Access profile picture
                    var file = provider.FileData[0]; // Only one file is uploaded
                    var name = file.Headers.ContentDisposition.FileName;

                    // Just get extension of file name
                    name = name.Substring(1, name.Length - 2);
                    var fileExtension = Path.GetExtension(name).ToLower(); // Get the file extension

                    // Upload file to Azure Blob Storage
                    (bool success, string blobUrl) = await AzureBlobStorage.UploadFileToBlobStorage(file.LocalFileName, fileExtension, "images");

                    // If image uploaded get image url in blob storage
                    // and put into post table
                    if (!success)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        // Save blob url to post 
                        post.ImageBlobUrl = blobUrl;
                    }
                }
                else
                {
                    // Image is optional
                    post.ImageBlobUrl = "";
                }

                // Put post into table
                bool insert_result = await InsertPost.Execute(AzureTableStorageCloudAccount.GetCloudTable("posts"), post);

                if (insert_result)
                {
                    // User was successfully added to the table
                    return Ok(post.Id); // Return 200 OK and post id
                }
                else
                {
                    // Post was not successfully added to the table
                    // Delete the image associated with the post
                    if (has_image)
                    {
                        await AzureBlobStorage.RemoveFileFromBlobStorage(post.ImageBlobUrl);
                    }

                    return BadRequest("Post creation failed"); // Return 400 Bad Request with an error message
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            finally
            {
                // Delete the saved file from the App_Data directory
                foreach (var fileData in provider.FileData)
                {
                    var localFilePath = fileData.LocalFileName;
                    File.Delete(localFilePath);
                }
            }
        }
        #endregion

        #region GET
        /// <summary>
        /// Get a specific post by post id
        /// </summary>
        /// <returns>
        /// An <see cref="IHttpActionResult"/> representing the result of the operation with post object.
        /// </returns>
        [Route("{postId}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string postId)
        {
            try
            {
                if (postId == null || postId == "")
                {
                    return BadRequest();
                }

                List<Comment> comments = await ReadComments.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), postId);
                Post post = await ReadPost.Run(AzureTableStorageCloudAccount.GetCloudTable("posts"), "Post", postId);

                if (post == null)
                {
                    return NotFound();
                }

                // Create DTO object of post with comments
                GetPost getPost = new GetPost(post.Id, post.Author, post.Title, post.Content, post.HasImage, post.ImageBlobUrl, comments.OrderByDescending(x => x.Timestamp).ToList());

                return Ok(getPost);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes a post with the specified post ID.
        /// </summary>
        /// <param name="postId">The ID of the post to delete.</param>
        /// <returns>
        /// IHttpActionResult representing the result of the deletion operation.
        /// Returns Ok() if the post and its associated comments were successfully deleted.
        /// Returns Unauthorized() if the request is not authorized.
        /// Returns BadRequest() if the comments associated with the post could not be deleted.
        /// Returns NotFound() if the post to delete is not found.
        /// Returns InternalServerError() if an unexpected error occurs during the deletion process.
        /// </returns>
        [HttpDelete]
        [Route("{postId}")]
        [JwtAuthenticationFilter] // Requires JWT authentication
        public async Task<IHttpActionResult> Delete(string postId)
        {
            try
            {
                // Retrieve the post author by ID
                string author = await ReadPostAuthor.Execute(AzureTableStorageCloudAccount.GetCloudTable("posts"), postId);

                // Only the author of the post can delete it
                if (!ResourceGuard.RunCheck(ActionContext, author))
                {
                    // Return unauthorized if the request is not authorized
                    return Unauthorized();
                }

                // Delete the post
                bool deleteResult = await DeletePost.Execute(AzureTableStorageCloudAccount.GetCloudTable("posts"), postId);

                if (deleteResult)
                {
                    // Delete all comments associated with the post
                    bool deleteCommentsResult = await RemoveComments.Execute(AzureTableStorageCloudAccount.GetCloudTable("comments"), postId);

                    if (deleteCommentsResult)
                    {
                        return Ok();
                    }
                    else
                    {
                        // Return BadRequest if comments associated with the post could not be deleted
                        return BadRequest();
                    }
                }
                else
                {
                    // Return NotFound if the post to delete is not found
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                // Return InternalServerError if an unexpected error occurs during the deletion process
                return InternalServerError(e);
            }
        }
        #endregion

        #region GET ALL POSTS WITH OPTIONAL SEARCH QUERY
        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> Posts(int pageSize = 10, int pageNumber = 1, string searchKeywords = "")
        {
            try
            {
                // Retrieve cloud table instance
                var cloudTable = AzureTableStorageCloudAccount.GetCloudTable("posts");

                // Call the Execute method of ReadPosts class to get paginated and searched posts
                var posts = await ReadPosts.Execute(cloudTable, searchKeywords, pageSize, pageNumber);

                return Ok(posts);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        #endregion
    }
}
