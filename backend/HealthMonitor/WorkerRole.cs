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

        private IHealthMonitoring proxy;
        public void Connect()
        {
            var binding = new NetTcpBinding();
            ChannelFactory<IHealthMonitoring> factory = new
            ChannelFactory<IHealthMonitoring>(binding, new
            EndpointAddress("net.tcp://localhost:6000/HealthMonitoring"));
            proxy = factory.CreateChannel();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Connect();
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
