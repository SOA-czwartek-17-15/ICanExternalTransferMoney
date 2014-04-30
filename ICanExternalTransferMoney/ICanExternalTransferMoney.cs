using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    /*
    * Przelewy zewnętrzne
    **/
    [ServiceContract]
    public interface ICanExternalTransferMoney
    {
        [OperationContract]
        long ReceiveExternalMoney(Guid from, string to, double howMany);
        [OperationContract]
        long SendExternalMoney(string from, Guid to, double howMany);
    }
}