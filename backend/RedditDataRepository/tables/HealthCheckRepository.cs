using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RedditDataRepository.tables.entities;
using RedditDataRepository.tables.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.tables
{
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        #region Init
        public HealthCheckRepository()
        {
            //_storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            _storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("HealthCheckTable");
            _table.CreateIfNotExists();
        }
        #endregion

        #region CRUD Operations
        public bool Create(HealthCheck healthCheck)
        {
            if (healthCheck == null)
                return false;

            try
            {
                TableOperation insertOperation = TableOperation.Insert(healthCheck);
                _table.Execute(insertOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public HealthCheck Read(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
                return new HealthCheck();

            try
            {
                var healthCheck = (from g in _table.CreateQuery<HealthCheck>()
                               where g.PartitionKey == "HealthCheck" && g.RowKey == dateTime
                               select g).FirstOrDefault();
                return healthCheck ?? new HealthCheck();
            }
            catch (Exception)
            {
                return new HealthCheck();
            }
        }

        public IQueryable<HealthCheck> ReadAll()
        {
            try
            {
                var results = from g in _table.CreateQuery<HealthCheck>()
                              where g.PartitionKey == "HealthCheck"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<HealthCheck>().AsQueryable();
            }
        }

        public bool Update(string dateTime, HealthCheck healthCheck)
        {
            if (string.IsNullOrEmpty(dateTime) || healthCheck == null)
                return false;

            try
            {
                var existingHealthCheck = (from g in _table.CreateQuery<HealthCheck>()
                                       where g.PartitionKey == "HealthCheck" && g.RowKey == dateTime
                                       select g).FirstOrDefault();

                if (existingHealthCheck != null)
                {
                    existingHealthCheck.Status = healthCheck.Status;
                    existingHealthCheck.Service = healthCheck.Service;

                    TableOperation updateOperation = TableOperation.Replace(existingHealthCheck);

                    _table.Execute(updateOperation);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(string dateTime)
        {
            if (string.IsNullOrEmpty(dateTime))
                return false;

            var healthCheckToDelete = (from g in _table.CreateQuery<HealthCheck>()
                                   where g.PartitionKey == "HealthCheck" && g.RowKey == dateTime
                                   select g).FirstOrDefault();

            if (healthCheckToDelete != null)
            {
                try
                {
                    TableOperation deleteOperation = TableOperation.Delete(healthCheckToDelete);

                    _table.Execute(deleteOperation);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public int GetCheckCount(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string startRowKey = startDate.ToString("yyyyMMddHHmmss") + startDate.Millisecond.ToString("000");
            string endRowKey = endDate.ToString("yyyyMMddHHmmss") + endDate.Millisecond.ToString("000");

            var query = new TableQuery<HealthCheck>()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "HealthCheck")
                    + " and " +
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey)
                    + " and " +
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey)
                );

            return _table.ExecuteQuery(query).Count();
        }

        public int GetOkCheckCount(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string startRowKey = startDate.ToString("yyyyMMddHHmmss") + startDate.Millisecond.ToString("000");
            string endRowKey = endDate.ToString("yyyyMMddHHmmss") + endDate.Millisecond.ToString("000");

            var query = new TableQuery<HealthCheck>()
                .Where(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "HealthCheck")
                    + " and " +
                    TableQuery.GenerateFilterCondition("Status", QueryComparisons.Equal, "OK")
                    + " and " +
                    TableQuery.GenerateFilterCondition("Service", QueryComparisons.Equal, "Reddit")
                    + " and " +
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey)
                    + " and " +
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey)
                );

            return _table.ExecuteQuery(query).Count();
        }
        #endregion
    }
}
