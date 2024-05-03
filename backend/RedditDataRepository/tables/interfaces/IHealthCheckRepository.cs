using RedditDataRepository.tables.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDataRepository.tables.interfaces
{
    public interface IHealthCheckRepository
    {
        bool Create(HealthCheck healthCheck);

        HealthCheck Read(string dateTime);

        IQueryable<HealthCheck> ReadAll();

        bool Update(string dateTime, HealthCheck healthCheck);

        bool Delete(string dateTime);

        int GetCheckCount(DateTimeOffset startDate, DateTimeOffset endDate);

        int GetOkCheckCount(DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
