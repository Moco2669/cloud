using RedditDataRepository.tables.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.tables.interfaces
{
    public interface IAlertEmailRepository
    {
        bool Create(AlertEmail alertEmail);

        AlertEmail Read(string id);

        IQueryable<AlertEmail> ReadAll();

        bool Update(string id, AlertEmail alertEmail);

        bool Delete(string id);
    }
}
