using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.classes.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.logs.Create
{
    public class InsertEmailLog
    {
        public static async Task<bool> Execute(CloudTable table, EmailLog log)
        {
            if(table == null || log == null)
            {
                return false;
            }
            try
            {
                // Create a TableOperation object to insert or replace the entity
                TableOperation tableOperation = TableOperation.InsertOrReplace(log);

                // Execute the operation asynchronously
                TableResult result = await table.ExecuteAsync(tableOperation);

                // Check if the operation was successful
                return result.HttpStatusCode == 200 || result.HttpStatusCode == 204;
            }
            catch
            {
                // An error occurred during insertion
                return false;
            }
        }
    }
}
