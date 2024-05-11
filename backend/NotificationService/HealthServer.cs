using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class HealthServer
    {
        private ServiceHost serviceHost;
        private int port;

        public HealthServer(int port)
        {
            this.port = port;
            Start();
        }

        public void Start()
        {
            serviceHost = new ServiceHost(typeof(HealthMonitoring));
            NetTcpBinding binding = new NetTcpBinding();
            serviceHost.AddServiceEndpoint(typeof(IHealthMonitoring), binding, new Uri($"net.tcp://localhost:{port}/HealthMonitoring"));
            serviceHost.Open();
            Console.WriteLine($"Server ready and waiting for requests on port {port}.");
        }
    }
}
