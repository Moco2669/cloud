using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace HealthMonitor
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private IHealthMonitoring proxy;
        private string redditServiceEndpoint;

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
            // dizvucem id instance da bi jedan healthMonitor pratio jedan RedditService a drugi drugi
            string roleId = RoleEnvironment.CurrentRoleInstance.Id;
            int startIndex = roleId.LastIndexOf("_IN_") + 4;
            int endIndex = roleId.Length;
            string instanceIndexStr = roleId.Substring(startIndex, endIndex - startIndex);

            int instanceIndex = int.Parse(instanceIndexStr);
            int port = 6000 + instanceIndex;
            redditServiceEndpoint = $"net.tcp://localhost:{port}/HealthMonitoring";

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("HealthMonitor has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitor is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("HealthMonitor has stopped");
        }

        public void Connect()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new ChannelFactory<IHealthMonitoring>(binding, new EndpointAddress(redditServiceEndpoint));
            proxy = factory.CreateChannel();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            Connect();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    proxy.IAmAlive();
                    Trace.TraceInformation("Service is alive.");
                }
                catch
                {
                    Trace.TraceWarning("Service not alive anymore!");
                }
                await Task.Delay(5000);
            }
        }
    }
}
