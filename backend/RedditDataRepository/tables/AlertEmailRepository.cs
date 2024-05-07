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
    public class AlertEmailRepository : IAlertEmailRepository
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;

        #region Init
        public AlertEmailRepository()
        {
            _storageAccount =
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new
            Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference("AlertEmailTable");
            _table.CreateIfNotExists();
        }
        #endregion

        #region CRUD Operations
        public bool Create(AlertEmail alertEmail)
        {
            if (alertEmail == null)
                return false;

            try
            {
                TableOperation insertOperation = TableOperation.Insert(alertEmail);
                _table.Execute(insertOperation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public AlertEmail Read(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new AlertEmail();

            try
            {
                var alertEmail = (from g in _table.CreateQuery<AlertEmail>()
                                   where g.PartitionKey == "AlertEmail" && g.RowKey == id
                                   select g).FirstOrDefault();
                return alertEmail ?? new AlertEmail();
            }
            catch (Exception)
            {
                return new AlertEmail();
            }
        }

        public IQueryable<AlertEmail> ReadAll()
        {
            try
            {
                var results = from g in _table.CreateQuery<AlertEmail>()
                              where g.PartitionKey == "AlertEmail"
                              select g;
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<AlertEmail>().AsQueryable();
            }
        }

        public bool Update(string id, AlertEmail alertEmail)
        {
            if (string.IsNullOrEmpty(id) || alertEmail == null)
                return false;

            try
            {
                var existingAlertEmail = (from g in _table.CreateQuery<AlertEmail>()
                                           where g.PartitionKey == "AlertEmail" && g.RowKey == id
                                           select g).FirstOrDefault();

                if (existingAlertEmail != null)
                {
                    existingAlertEmail.Email = alertEmail.Email;

                    TableOperation updateOperation = TableOperation.Replace(existingAlertEmail);

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

        public bool Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            var alertEmailToDelete = (from g in _table.CreateQuery<AlertEmail>()
                                       where g.PartitionKey == "AlertEmail" && g.RowKey == id
                                       select g).FirstOrDefault();

            if (alertEmailToDelete != null)
            {
                try
                {
                    TableOperation deleteOperation = TableOperation.Delete(alertEmailToDelete);

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
        #endregion
    }
}
