using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Net.Http;

namespace RedditDataRepository.classes.Votes
{
    /// <summary>
    /// Represents a vote cast by a user on a post.
    /// </summary>
    public class Vote : TableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the vote.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the post being voted on.
        /// </summary>
        public string PostId { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user who cast the vote.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what type of vote the user cast.
        /// </summary>
        public bool Voted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vote"/> class.
        /// </summary>
        public Vote() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vote"/> class with the specified parameters.
        /// </summary>
        /// <param name="email">The email address of the user who cast the vote.</param>
        /// <param name="postId">The identifier of the post being voted on.</param>
        /// <param name="voted">A value indicating what type of vote the user cast.</param>
        public Vote(string email, string postId, bool voted)
        {
            Id = Guid.NewGuid().ToString();
            PartitionKey = "Vote";
            RowKey = Id;
            Email = email;
            PostId = postId;
            Voted = voted;
        }
    }
}
