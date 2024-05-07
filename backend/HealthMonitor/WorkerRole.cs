using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.cloud.queue;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.queues;
using RedditDataRepository.tables;
using RedditDataRepository.tables.entities;

namespace HealthMonitor
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private List<string> redditServiceEndpoints = new List<string>();
        private static HealthCheckRepository repository = new HealthCheckRepository();
        private static AdminToolServer AdminToolServer;
        private int instanceIndex;

        public override void Run()
        {
            Trace.TraceInformation("HealthMonitor is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {

            string roleId = RoleEnvironment.CurrentRoleInstance.Id;
            int startIndex = roleId.LastIndexOf("_IN_") + 4;
            int endIndex = roleId.Length;
            string instanceIndexStr = roleId.Substring(startIndex, endIndex - startIndex);
            instanceIndex = int.Parse(instanceIndexStr);

            int portBase = 6000;
            int redditServiceCount = 2;
            for (int i = 0; i < redditServiceCount; i++)
            {
                int port = portBase + i;
                string endpoint = $"net.tcp://localhost:{port}/HealthMonitoring";
                redditServiceEndpoints.Add(endpoint);
            }

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            AdminToolServer = new AdminToolServer();
            AdminToolServer.Open();

            Trace.TraceInformation("HealthMonitor has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitor is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            AdminToolServer.Close();

            Trace.TraceInformation("HealthMonitor has stopped");
        }

        public void Connect(string endpoint)
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(binding, new EndpointAddress(endpoint));
            IHealthMonitoring proxy = factory.CreateChannel();
            CheckServiceHealth(proxy);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                if(instanceIndex == 0)
                {
                    foreach (var endpoint in redditServiceEndpoints)
                    {
                        Connect(endpoint);
                    }
                }

                await Task.Delay(new Random().Next(1000, 5001), cancellationToken);
            }
        }

        private void CheckServiceHealth(IHealthMonitoring proxy)
        {
            try
            {
                proxy.IAmAlive();
                LogHealthCheck("OK");
                Trace.TraceInformation("Service is alive.");
            }
            catch
            {
                LogHealthCheck("NOT_OK");
                Trace.TraceWarning("Service not alive anymore!");
                //CloudQueue queue = AzureQueueHelper.GetQueue("adminnotificationqueue");
                //AdminNotificationQueue.EnqueueMessage(queue, "Alert admin emails!");
            }
        }

        private void LogHealthCheck(string status)
        {
            string service = "";
            if(instanceIndex == 0)
            {
                service = "Reddit";
            }
            if(instanceIndex == 1)
            {
                service = "Notification";
            }

            DateTime timestamp = DateTime.UtcNow;
            HealthCheck healthCheckEntity = new HealthCheck(timestamp.ToString("yyyyMMddHHmmssfff"), status, service);
            repository.Create(healthCheckEntity);
        }
    }
}
