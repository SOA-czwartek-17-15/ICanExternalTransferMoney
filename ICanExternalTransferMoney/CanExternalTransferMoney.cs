using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using NHibernate;
using ICanExternalTransferMoney.Domain;
using System.ServiceModel;

namespace ICanExternalTransferMoney
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CanExternalTransferMoney : Contracts.ICanExternalTransferMoney
    {
        public IAccountRepository AccountRepository { set; get; }
        public ISession Session { set; get; }

        /// <summary>
        /// Konstruktor implementacji serwisu
        /// </summary>
        /// <param name="_Session">Sesja NHibernate</param>
        public CanExternalTransferMoney(ISession _Session){ Session = _Session; }

        public Guid ReceiveExternalMoney(string from, Guid to, double howMany)
        {
            if (AccountRepository == null) return Guid.Empty;
            //TODO jak będzie AccountRepository
            //AccountRepository.ChangeBalance(to, howMany);
            string nrKonta = null;//AccountRepository.GetAccountInformation(to).AccountNumber;

            //logi

            //Dodanie do bazy MySQL potwierdzenia operacji
            ITransaction transaction = Session.BeginTransaction();
            Potwierdzenie potwierdzenie = new Potwierdzenie("z zewnątrz", from, nrKonta, howMany);
            Session.Save(potwierdzenie);
            transaction.Commit();

            return potwierdzenie.IdPotwierdzenia;
        }

        public Guid SendExternalMoney(Guid from, string to, double howMany)
        {
            if (AccountRepository == null) return Guid.Empty;
            //TODO jak będzie AccountRepository
            //AccountRepository.ChangeBalance(from, howMany);
            string nrKonta = null;//AccountRepository.GetAccountInformation(from).AccountNumber;

            //logi

            //Dodanie do bazy MySQL potwierdzenia operacji
            ITransaction transaction = Session.BeginTransaction();
            Potwierdzenie potwierdzenie = new Potwierdzenie("na zewnątrz", nrKonta, to, howMany);
            Session.Save(potwierdzenie);
            transaction.Commit();

            return potwierdzenie.IdPotwierdzenia;
        }
    }
}
