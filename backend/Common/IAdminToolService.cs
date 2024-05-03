using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IAdminToolService
    {
        [OperationContract]
        bool AddEmail(AlertEmailDTO alertEmail);

        [OperationContract]
        List<AlertEmailDTO> ReadAllEmails();

        [OperationContract]
        bool RemoveEmail(string Id);
    }
}
