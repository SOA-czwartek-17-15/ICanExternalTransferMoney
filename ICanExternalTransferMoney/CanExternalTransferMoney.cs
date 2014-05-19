using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using NHibernate;
using ICanExternalTransferMoney.Domain;
using System.ServiceModel;
using log4net;
using log4net.Config;
using Contracts;

namespace ICanExternalTransferMoney
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CanExternalTransferMoney : Contracts.ICanExternalTransferMoney
    {
        public IAccountRepository AccountRepository { set; get; }
        public ISession Session { set; get; }
        private static readonly ILog log = LogManager.GetLogger(typeof(CanExternalTransferMoney));

        /// <summary>
        /// Konstruktor implementacji serwisu
        /// </summary>
        /// <param name="_Session">Sesja NHibernate</param>
        public CanExternalTransferMoney(ISession _Session){ Session = _Session; }

        public Guid ReceiveExternalMoney(string from, Guid to, double howMany)
        {
            if (AccountRepository == null) return Guid.Empty;
            Account toAccount = AccountRepository.GetAccountById(to);
            string nrKonta = toAccount.accountNumber;
            if (AccountRepository.ChangeAccountBalance(to, toAccount.money + (long)howMany))//nie wiem czemu long jest w interfejsie o.O
            {
                //log
                log.InfoFormat("Otrzymano: {0} od: {1} do: {2}({3})", howMany, from, nrKonta, to);

                //Dodanie do bazy MySQL potwierdzenia operacji
                ITransaction transaction = Session.BeginTransaction();
                Potwierdzenie potwierdzenie = new Potwierdzenie("z zewnątrz", from, nrKonta, howMany);
                Session.Save(potwierdzenie);
                transaction.Commit();

                return potwierdzenie.IdPotwierdzenia;
            }
            log.ErrorFormat("NIE Otrzymano: {0} od: {1} do: {2}({3})", howMany, from, nrKonta, to);
            return Guid.Empty;
        }

        public Guid SendExternalMoney(Guid from, string to, double howMany)
        {
            if (AccountRepository == null) return Guid.Empty;
            Account fromAccount = AccountRepository.GetAccountById(from);
            string nrKonta = fromAccount.accountNumber;
            Contracts.Account nowy = new Contracts.Account();
            if (AccountRepository.ChangeAccountBalance(from, fromAccount.money + (long)howMany)) //nie wiem czemu long jest w interfejsie o.O
            {
                //log
                log.InfoFormat("Wysłano: {0} do: {1} od: {2}({3})", howMany, to, nrKonta, from);

                //Dodanie do bazy MySQL potwierdzenia operacji
                ITransaction transaction = Session.BeginTransaction();
                Potwierdzenie potwierdzenie = new Potwierdzenie("na zewnątrz", nrKonta, to, howMany);
                Session.Save(potwierdzenie);
                transaction.Commit();

                return potwierdzenie.IdPotwierdzenia;
            }
            log.ErrorFormat("NIE Wysłano: {0} do: {1} od: {2}({3})", howMany, to, nrKonta, from);
            return Guid.Empty;
        }
    }
}
