using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.tables.entities
{
    public class HealthCheck : TableEntity
    {
        public string Status { get; set; }

        public string Service { get; set; }

        public HealthCheck(string dateTime, string status, string service)
        {
            PartitionKey = "HealthCheck";
            RowKey = dateTime;
            Status = status;
            Service = service;
        }

        public HealthCheck()
        {
        }
    }
}
