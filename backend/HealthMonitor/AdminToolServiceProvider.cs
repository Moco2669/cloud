using Common;
using RedditDataRepository.tables;
using RedditDataRepository.tables.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor
{
    public class AdminToolServiceProvider : IAdminToolService
    {
        private static AlertEmailRepository repository = new AlertEmailRepository();

        public bool AddEmail(AlertEmailDTO alertEmail)
        {
            return repository.Create(new AlertEmail(alertEmail.Email, alertEmail.Id));
        }

        public List<AlertEmailDTO> ReadAllEmails()
        {
            return repository.ReadAll().Select(alertEmail => new AlertEmailDTO(alertEmail.RowKey, alertEmail.Email)).ToList();
        }

        public bool RemoveEmail(string id)
        {
            return repository.Delete(id);
        }
    }
}
