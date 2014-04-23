using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ICanExternalTransferMoney
{
    [ServiceContract]
    public interface ICanExternalTransferMoney
    {
        [OperationContract]
        bool ReceiveExternalMoney(Account from, long to, double howMany);
        [OperationContract]
        bool SendExternalMoney(long from, Account to, double howMany);
    }
}
