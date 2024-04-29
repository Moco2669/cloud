using Common.cloud.account;
using RedditDataRepository.classes.Comments;
using RedditDataRepository.classes.Posts;
using RedditDataRepository.comments.Read;
using RedditDataRepository.posts.Read;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
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
            emails = await ReadSubscriptions.Run(AzureTableStorageCloudAccount.GetCloudTable("subscriptions"), postId);
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

        public static async Task SendEmail(string email)
        {
            string domain = ".mailgun.org";
            string auth = "";
            string data = "";
            string jsonContent = "{" +
                "" +
                "}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"api:\"\"")));

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("from", $"Excited User <mailgun@{domain}>"),
                new KeyValuePair<string, string>("to", email),
                new KeyValuePair<string, string>("subject", "You missed on Le Reddit"),
                new KeyValuePair<string, string>("text", "nesto")
            });

                var response = await client.PostAsync($"https://api.mailgun.net/v3/{domain}/messages", content);

                if (response.IsSuccessStatusCode)
                {
                    Trace.TraceInformation("Email sent successfully.");
                }
                else
                {
                    Trace.TraceError($"Failed to send email. Status code: {response.StatusCode}");
                }
            }
        }
    }

}
