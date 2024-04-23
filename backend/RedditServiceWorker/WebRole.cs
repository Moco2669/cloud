using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;

namespace RedditServiceWorker
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            StartHealthServer();
            return base.OnStart();
        }

        private void StartHealthServer()
        {
            try
            {
                new HealthServer();
                Trace.TraceInformation("HealthMonitor server is running");
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Error starting WCF service!" + e.Message);
            }
        }
    }
}
