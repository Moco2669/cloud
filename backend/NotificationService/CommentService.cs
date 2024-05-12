using Common.cloud.account;
using RedditDataRepository.classes.Comments;
using RedditDataRepository.classes.Posts;
using RedditDataRepository.comments.Read;
using RedditDataRepository.posts.Read;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
//using System.Text.Json;
//using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Mailjet.Client;
using Mailjet.Client.Resources;
using SendGrid;
using SendGrid.Helpers.Mail;

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

        public static async Task<bool> SendEmail(string email, string commentText)
        {
            Trace.TraceError("Trying to send email to " + email);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string api_key = "API_KEY";
            string api_secret = "API_SECRET";
            var client = new SendGridClient(api_key);
            var fromEmail = new EmailAddress("LeReddit@lereddit.com", "LeReddit");
            var toEmail = new EmailAddress(email, email.Split('@')[0]);
            var text = commentText;
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, "You missed on LeReddit", text, text);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;

            // MAILJET
            /*MailjetClient client = new MailjetClient(api_key, api_secret){};
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
                new JObject {
                    {
                        "From",
                        new JObject {
                            {"Email", "momcilo.micic.business@gmail.com"},
                            {"Name", "Momcilo"}
                        }
                    },
                    {
                        "To",
                        new JArray {
                            new JObject {
                                {"Email", email},
                                {"Name", "LeReddit user"}
                            }
                        }
                    },
                    {"Subject", "Greetings from Mailjet."},
                    {"TextPart", "My first Mailjet email"},
                    {"HTMLPart", "<h3>Dear passenger 1, welcome to <a href='https://www.mailjet.com/'>Mailjet</a>!</h3><br />May the delivery force be with you!"},
                    {"CustomID", "AppGettingStartedTest"}
                }
             });
            Trace.TraceError(request.Body.ToString());
            MailjetResponse response = await client.PostAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Trace.TraceError(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                Trace.TraceError(response.GetData().ToString());
                return true;
            }
            else
            {
                Trace.TraceError(string.Format("StatusCode: {0}\n", response.StatusCode));
                Trace.TraceError(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                Trace.TraceError(response.GetData().ToString());
                Trace.TraceError(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                return false;
            }*/


            // MAILJET USING MY OWN IMPLEMENTATION
            /*using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{api_key}:{api_secret}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Messages", "[{\"From\":{\"Email\":\"momcilo.micic.business@gmail.com\",\"Name\":\"Momcilo\"},"+
                    "\"To\":[{\"Email\":\""+email+"\",\"Name\":\"LeReddit recepient\"}],"+
                    "\"Subject\":\"You missed on LeReddit\""+
                    ",\"TextPart\":\"You missed on LeReddit\","+
                    "\"HTMLPart\":\"<h3>Dear passenger 1, welcome to <a href='https://www.mailjet.com/'>Mailjet</a>!</h3><br />May the delivery force be with you!\","+
                    "\"CustomID\":\"AppGettingStartedTest\"}]")
                });

                Trace.TraceError(await content.ReadAsStringAsync());

                var response = await client.PostAsync($"https://api.mailjet.com/v3.1/send", content);

                if (response.IsSuccessStatusCode)
                {
                    Trace.TraceError("Email sent successfully.");
                    return true;
                }
                else
                {
                    Trace.TraceError($"Failed to send email. Status code: {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    return false;
                }
            }*/
        }
    }

}
