using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace RedditServiceWorker
{
    public class HealthServer
    {
        private ServiceHost serviceHost;
        public HealthServer()
        {
            Start();
        }
        public void Start()
        {
            serviceHost = new ServiceHost(typeof(HealthMonitoring));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new
            Uri("net.tcp://localhost:6000/HealthMonitoring"));
            serviceHost.Open();
            Console.WriteLine("Server ready and waiting for requests.");
        }
    }
}