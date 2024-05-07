using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.tables.entities
{
    public class AlertEmail : TableEntity
    {
        public string Email { get; set; }

        public AlertEmail(string email, string id)
        {
            PartitionKey = "AlertEmail";
            RowKey = id;
            Email = email;
        }

        public AlertEmail()
        {
        }
    }
}
