using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditServiceWorker
{
    public class HealthMonitoring : IHealthMonitoring
    {
        public void IAmAlive()
        {
            Console.WriteLine("ZIV SAM");
        }
    }
}