﻿using Common.cloud.account;
using RedditDataRepository.users.Read;
using System.Threading.Tasks;
using System.Web.Http;

namespace RedditServiceWorker.Controllers
{
    [RoutePrefix("api/images")]
    public class ImagesController : ApiController
    {
        #region GET PROFILE PICTURE
        /// <summary>
        /// Retrieves the profile picture of a user based on their email.
        /// </summary>
        /// <param name="email">The email address of the user whose profile picture is to be retrieved.</param>
        /// <returns>
        /// Returns the profile picture URL of the user if found.
        /// If the user is not found, appropriate HTTP status codes are returned.
        /// </returns>
        [Route("{email}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetProfilePicture(string email)
        {
            // Retrieve the user's profile picture URL from the data repository
            string picture = await ReadUserProfilePicture.Run(AzureTableStorageCloudAccount.GetCloudTable("users"), "User", email);

            // Return the user's profile picture if found
            if (picture != null)
            {
                return Ok(picture);
            }
            else
            {
                // Return 404 Not Found if the user is not found
                return NotFound();
            }
        }
        #endregion
    }
}
