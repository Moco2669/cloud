using Common.cloud.queue;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using RedditDataRepository.queues;
using Common.cloud.account;
using RedditDataRepository.logs.Create;
using RedditDataRepository.classes.Logs;
using RedditDataRepository.comments.Read;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public async override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            CloudQueue queue = AzureQueueHelper.GetQueue("notifications");
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token, queue).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            AzureTableStorageCloudAccount account = new AzureTableStorageCloudAccount();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken, CloudQueue queue)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                // Trace.TraceInformation("Working");

                string commentId = NotificationQueue.DequeueComment(queue);
                if (commentId == null)
                {
                    await Task.Delay(1000);
                    continue;
                }
                // Send notifications to email
                List<string> emails = await CommentService.GetPostEmails(commentId);
                string commentText = (await ReadComment.Run(AzureTableStorageCloudAccount.GetCloudTable("comments"), commentId)).Content;
                int numOfEmailsSent = 0;
                foreach (string email in emails)
                {
                    if (await CommentService.SendEmail(email, commentText))
                    {
                        ++numOfEmailsSent;
                    }
                }
                // Save date and time and number of emails sent
                if (!(await InsertEmailLog.Execute(AzureTableStorageCloudAccount.GetCloudTable("emailLogs"), new EmailLog(DateTime.Now, commentId, numOfEmailsSent))))
                {
                    Trace.TraceError("Error inserting email log into table.");
                }
            }
        }
    }
}
