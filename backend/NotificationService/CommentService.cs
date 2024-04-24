using Common.cloud.account;
using RedditDataRepository.classes.Comments;
using RedditDataRepository.classes.Posts;
using RedditDataRepository.comments.Read;
using RedditDataRepository.posts.Read;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class CommentService
    {
        public static async Task<List<string>> GetPostEmails(string commentId)
        {
            List<string> emails = new List<string>();
            string postId = await getPostId(commentId);
            if (postId == null) return emails;
            emails = ReadSubscriptions.Run(AzureTableStorageCloudAccount.GetCloudTable("subscriptions"), postId);
            return emails;
        }
        private static async Task<string> getPostId(string commentId)
        {
            Comment comment = await ReadComment.Run(AzureTableStorageCloudAccount.GetCloudTable("comments"), commentId);
            if (comment == null) return null;
            Post post = await ReadPost.Run(AzureTableStorageCloudAccount.GetCloudTable("posts"), "Post", comment.PostId);
            if (post == null) return null;
            return post.Id;
        }
    }

}
