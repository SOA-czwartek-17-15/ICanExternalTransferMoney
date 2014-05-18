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
        private Account acc;
        public AccountRepository()
        {
            acc = new Account();
            acc.Money = 15;
            acc.AccountNumber = "123456";
        }

        public long CreateAccount(int clientId, Account details)
        {
            return 0;
        }

        public Account GetAccountInformation(string accountNumber)
        {
            return acc;
        }

        public Account GetAccountById(Guid accountId)
        {
            return acc;
        }

        public bool ChangeAccountBalance(Guid accountId, long amount)
        {
            Console.WriteLine("CHANGE: {0} - amount {1}",accountId,amount);
            return true;
        }
    }
}
