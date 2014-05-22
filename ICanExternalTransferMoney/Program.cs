using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using NHibernate;
using NHibernate.Cfg;
using ICanExternalTransferMoney.Domain;
using NHibernate.Tool.hbm2ddl;
using Contracts;
using System.Timers;
using log4net;
using log4net.Config;
using System.IO;

namespace ICanExternalTransferMoney
{
    class Program
    {
        private IServiceRepository serviceRepo;
        private CanExternalTransferMoney transfer;
        private string accountRepositoryAddress = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private ChannelFactory<IServiceRepository> servCF;
        private string serviceAdress = ConfigurationManager.AppSettings["serviceAddress"];

        private bool registerdOnServiceRepo = false;

        static void Main(string[] args) 
        {
            try{ new Program(); }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        public Program() 
        {
            //Otwarcie sesji NHibernate
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();
            config.AddAssembly(typeof(Potwierdzenie).Assembly);
            new SchemaExport(config).Execute(false, false, false); //Drugi na true gdy chcemy dropTable robić przy każdym uruchomieniu, false gdy mamy już uworzoną tabele
            ISessionFactory factory = config.BuildSessionFactory();
            ISession session = factory.OpenSession();

            //---------log----------
            Console.WriteLine("NHibernate is opened");
            log.Info("NHibernate is opened");
            //---------log----------

            //Utworzenie Serwisu ICanExternalTransferMoney
            transfer = new CanExternalTransferMoney(session);
            var sh = new ServiceHost(transfer, new Uri[] { new Uri(serviceAdress) });
            NetTcpBinding bindingOUT = new NetTcpBinding(SecurityMode.None);
            sh.AddServiceEndpoint(typeof(Contracts.ICanExternalTransferMoney), bindingOUT, serviceAdress);
            sh.Open();

            //---------log----------
            Console.WriteLine("Service has been made");
            log.Info("Service has been made");
            //---------log----------

            //Wyciąganie adresu ServiceRepository z App.config i uzyskanie ServiceRepo
            string serviceRepositoryAddress = ConfigurationManager.AppSettings["serviceRepositoryAddress"];
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            servCF = new ChannelFactory<IServiceRepository>(binding, new EndpointAddress(serviceRepositoryAddress));

            //timer
            Timer timer = new Timer();
            timer.Interval = Double.Parse(ConfigurationManager.AppSettings["aliveSignalDelay"]);
            timer.Elapsed += new ElapsedEventHandler(TimerOnTick);
            timer.Start();
            TimerOnTick(null,null);

            //---------log----------
            Console.WriteLine("Timer started");
            log.Info("Timer started");
            //---------log----------

            //ReadLine kończący program
            Console.WriteLine("ICanExternalTransferMoney is on!");
            Console.WriteLine("Kliknij Enter, aby wyłączyć serwis...");
            Console.ReadLine();

            //Zamknięcie sesji nhibernate
            session.Close();
            //---------log----------
            Console.WriteLine("NHibernate is not up anymore");
            log.Info("NHibernate is not up anymore");
            //---------log----------

            timer.Stop();

            //---------log----------
            Console.WriteLine("Timer stopped");
            log.Info("Timer stopped");
            //---------log----------

            try
            {
                if (registerdOnServiceRepo)
                {
                    serviceRepo = servCF.CreateChannel();
                    serviceRepo.Unregister("ICanExternalTransferMoney");

                    //---------log----------
                    Console.WriteLine("Unregistered in IServiceRepository");
                    log.Info("Unregistered in IServiceRepository");
                    //---------log----------
                }
            }
            catch (EndpointNotFoundException ex)
            {
                //---------log----------
                Console.WriteLine("Unregister failed, because Service is dead.");
                log.Info("Unregister failed, because Service is dead.");
                //---------log----------
            }

        }

        /// <summary>
        /// Wysłanie sygnału Alive do ServiceRepository oraz sprawdzenie 
        /// czy serwisy z których korzystamy działają/nie zmieniły adresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOnTick(object sender, EventArgs e)
        {
           try
           {
                //Rejestracja Serwisu w ServiceRepository i odpalenie timera
               if (!registerdOnServiceRepo)
               {
                   serviceRepo = servCF.CreateChannel();
                   serviceRepo.RegisterService("ICanExternalTransferMoney", serviceAdress);
                   registerdOnServiceRepo = true;

                   //---------log----------
                   Console.WriteLine("Service has been registered");
                   log.Info("Service has been registered");
                   //---------log----------
               }

               try
               {
                   serviceRepo = servCF.CreateChannel();
                   serviceRepo.Alive("ICanExternalTransferMoney");
                   Console.Write(".");

                   serviceRepo = servCF.CreateChannel();
                   string address = serviceRepo.GetServiceLocation("IAccountService");
                   if (address == null || !address.Equals(accountRepositoryAddress))
                   {
                       if (address != null)
                       {
                           accountRepositoryAddress = address;
                           NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                           ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(binding, new EndpointAddress(accountRepositoryAddress));
                           IAccountRepository accountRepository = cf.CreateChannel();
                           transfer.AccountRepoChannelFactory = cf;
                           transfer.AccountRepository = accountRepository;

                           //---------log----------
                           log.InfoFormat("New IAccountRepository address: {0}", address);
                           Console.WriteLine("New IAccountRepository address: {0}", address);
                           //---------log----------
                       }
                       else
                       {
                           transfer.AccountRepository = null;
                           accountRepositoryAddress = null;

                           //---------log----------
                           log.InfoFormat("AccountRepo not registered on ServiceRepo");
                           Console.WriteLine("\nAccountRepo not registered on ServiceRepo");
                           //---------log----------
                       }
                   }
               }
               catch (EndpointNotFoundException ex)
               {
                   //---------log----------
                   Console.WriteLine("\nAlive signal not send. ServiceRepo is dead!");
                   log.Error("Alive signal not send. ServiceRepo is dead!");
                   //---------log----------
                   registerdOnServiceRepo = false;
               }
            }
            catch (EndpointNotFoundException ex) {
                //---------log----------
                Console.WriteLine("ServiceRepository not found. Retry after 3s.");
                log.Error("ServiceRepository not found. Retry after 3s.");
                //---------log----------
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                log.Error(ex.Message);
            }
        }
    }
}


