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
        public ChannelFactory<IAccountRepository> AccountRepoChannelFactory { set; get; }
        private static readonly ILog log = LogManager.GetLogger(typeof(CanExternalTransferMoney));
        public DAO DataAccessObject{ set; get;}

        /// <summary>
        /// Konstruktor implementacji serwisu
        /// </summary>
        /// <param name="_Session">Sesja NHibernate</param>
        public CanExternalTransferMoney(DAO _dao)
        {
            DataAccessObject = _dao; 
        }

        public Guid ReceiveExternalMoney(string from, Guid to, double howMany)
        {
            try
            {
                if (AccountRepository == null)
                {
                    //---------log----------
                    log.ErrorFormat("AccountRepo is null - NIE Otrzymano: {0} od: {1} do: {2}", howMany, from, to);
                    Console.WriteLine("AccountRepo is null - NIE Otrzymano: {0} od: {1} do: {2}", howMany, from, to);
                    //---------log----------
                    return Guid.Empty;
                }
                AccountRepository = AccountRepoChannelFactory.CreateChannel();
                Account toAccount = AccountRepository.GetAccountById(to);
                string nrKonta = toAccount.AccountNumber;
                AccountRepository = AccountRepoChannelFactory.CreateChannel();
                if (AccountRepository.ChangeAccountBalance(to, toAccount.Money + (long)howMany))//nie wiem czemu long jest w interfejsie o.O
                {
                    //---------log----------
                    Console.WriteLine("\nOtrzymano: {0} od: {1} do: {2}({3})", howMany, from, nrKonta, to);
                    log.InfoFormat("Otrzymano: {0} od: {1} do: {2}({3})", howMany, from, nrKonta, to);
                    //---------log----------

                    //Dodanie do bazy MySQL potwierdzenia operacji
                    Potwierdzenie potwierdzenie = new Potwierdzenie("z zewnątrz", from, nrKonta, howMany);
                    if (!DataAccessObject.SavePotwierdzenieToBase(potwierdzenie))
                    {
                        //---------log----------
                        Console.WriteLine("Nie zapisano do bazy!");
                        log.Error("Nie zapisano do bazy!");
                        //---------log----------
                    }

                    return potwierdzenie.IdPotwierdzenia;
                }
                log.ErrorFormat("NIE Otrzymano: {0} od: {1} do: {2}({3})", howMany, from, nrKonta, to);
                return Guid.Empty;
            }
            catch (EndpointNotFoundException ex)
            {
                //---------log----------
                Console.WriteLine("AccountRepo is dead!");
                log.Error("AccountRepo is dead!");
                log.ErrorFormat("NIE Otrzymano: {0} od: {1} do: {2}", howMany, from, to);
                //---------log----------
                return Guid.Empty;
            }
        }

        public Guid SendExternalMoney(Guid from, string to, double howMany)
        {
            try
            {
                if (AccountRepository == null)
                {
                    //---------log----------
                    log.ErrorFormat("AccountRepo is null - NIE Wysłano: {0} do: {1} od: {2}", howMany, to, from);
                    Console.WriteLine("AccountRepo is null - NIE Wysłano: {0} do: {1} od: {2}", howMany, to, from);
                    //---------log----------
                    return Guid.Empty;
                }
                AccountRepository = AccountRepoChannelFactory.CreateChannel();
                Account fromAccount = AccountRepository.GetAccountById(from);
                string nrKonta = fromAccount.AccountNumber;
                Contracts.Account nowy = new Contracts.Account();
                AccountRepository = AccountRepoChannelFactory.CreateChannel();
                if (AccountRepository.ChangeAccountBalance(from, fromAccount.Money + (long)howMany)) //nie wiem czemu long jest w interfejsie o.O
                {
                    //---------log----------
                    log.InfoFormat("Wysłano: {0} do: {1} od: {2}({3})", howMany, to, nrKonta, from);
                    Console.WriteLine("\nWysłano: {0} do: {1} od: {2}({3})", howMany, to, nrKonta, from);
                    //---------log----------

                    //Dodanie do bazy MySQL potwierdzenia operacji
                    
                    Potwierdzenie potwierdzenie = new Potwierdzenie("na zewnątrz", nrKonta, to, howMany);
                    if (!DataAccessObject.SavePotwierdzenieToBase(potwierdzenie))
                    {
                        //---------log----------
                        Console.WriteLine("Nie zapisano do bazy!");
                        log.Error("Nie zapisano do bazy!");
                        //---------log----------
                    }

                    return potwierdzenie.IdPotwierdzenia;
                }
                log.ErrorFormat("NIE Wysłano: {0} do: {1} od: {2}({3})", howMany, to, nrKonta, from);
                return Guid.Empty;
            }
            catch (EndpointNotFoundException ex)
            {
                //---------log----------
                Console.WriteLine("AccountRepo is dead!");
                log.Error("AccountRepo is dead!");
                log.ErrorFormat("NIE Wysłano: {0} do: {1} od: {2}", howMany, to, from);
                //---------log----------
                return Guid.Empty;
            }
        }
    }
}
