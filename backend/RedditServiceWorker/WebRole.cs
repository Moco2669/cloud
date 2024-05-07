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
                // isto ko kod healthmonitora
                string roleId = RoleEnvironment.CurrentRoleInstance.Id;
                int startIndex = roleId.LastIndexOf("_IN_") + 4;
                int endIndex = roleId.Length;
                string instanceIndexStr = roleId.Substring(startIndex, endIndex - startIndex);

                int instanceIndex = int.Parse(instanceIndexStr);
                int port = 6000 + instanceIndex;
                new HealthServer(port);
                Trace.TraceInformation($"HealthMonitor server for RedditService {instanceIndex + 1} is running on port {port}");
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Error starting WCF service!" + e.Message);
            }
        }
    }
}
