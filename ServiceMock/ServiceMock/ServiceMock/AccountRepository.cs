using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.ServiceModel;

namespace ServiceMock
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class AccountRepository : IAccountRepository
    {
        public long CreateAccount(int clientId, Account details)
        {
            return 0;
        }

        public Account GetAccountInformation(string accountNumber)
        {

        }

        public Account GetAccountById(Guid accountId)
        {
        }

        public bool ChangeAccountBalance(Guid accountId, long amount)
        {
            Console.WriteLine("CHANGE: {0} - amount {1}",accountId,amount);
            return true;
        }
    }
}
