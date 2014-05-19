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
        //TODO logowanie - log4net

        private IServiceRepository serviceRepo;
        private CanExternalTransferMoney transfer;
        private string accountRepositoryAddress = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

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
            //Wyciąganie adresu ServiceRepository z App.config i uzyskanie ServiceRepo
            string serviceRepositoryAddress = ConfigurationManager.AppSettings["serviceRepositoryAddress"];
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(binding, new EndpointAddress(serviceRepositoryAddress));
            serviceRepo = cf.CreateChannel();

            //---------log----------
            log.Info("Got App.config adress and servicerepo");
            //---------log----------


            //Otwarcie sesji NHibernate
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();
            config.AddAssembly(typeof(Potwierdzenie).Assembly);
            new SchemaExport(config).Execute(false, false, false); //Drugi na true gdy chcemy dropTable robić przy każdym uruchomieniu, false gdy mamy już uworzoną tabele
            ISessionFactory factory = config.BuildSessionFactory();
            ISession session = factory.OpenSession();

            //---------log----------
            log.Info("NHibernate is opened");
            //---------log----------

            //Utworzenie Serwisu ICanExternalTransferMoney
            transfer = new CanExternalTransferMoney(session);
            string serviceAdress = ConfigurationManager.AppSettings["serviceAddress"];
            var sh = new ServiceHost(transfer, new Uri[] { new Uri(serviceAdress) });
            NetTcpBinding bindingOUT = new NetTcpBinding(SecurityMode.None);
            sh.AddServiceEndpoint(typeof(Contracts.ICanExternalTransferMoney), bindingOUT, serviceAdress);
            sh.Open();

            //---------log----------
            log.Info("Service has been made");
            //---------log----------

            //Rejestracja Serwisu w ServiceRepository i odpalenie timera
            serviceRepo.RegisterService("ICanExternalTransferMoney", serviceAdress);
            Timer timer = new Timer();
            timer.Interval = Double.Parse(ConfigurationManager.AppSettings["aliveSignalDelay"]);
            timer.Elapsed += new ElapsedEventHandler(TimerOnTick);
            timer.Start();
            TimerOnTick(null,null);

            //---------log----------
            log.Info("Service has been registered, timer is up");
            //---------log----------



            //Pewnie pójdzie do usunięcie WriteLine jak będą logi, ReadLine potrzebny
            Console.WriteLine("Service Repo: {0}", serviceRepositoryAddress);
            Console.WriteLine("Kliknij Enter, aby wyłączyć serwis...");
            Console.ReadLine();

            //Zamknięcie sesji nhibernate
            session.Close();
            timer.Stop();
            serviceRepo.Unregister("ICanExternalTransferMoney");

            //---------log----------
            log.Info("NHibernate is not up anymore");
            //---------log----------

        }

        /// <summary>
        /// Wysłanie sygnału Alive do ServiceRepository oraz sprawdzenie 
        /// czy serwisy z których korzystamy działają/nie zmieniły adresu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOnTick(object sender, EventArgs e)
        {
            serviceRepo.Alive("ICanExternalTransferMoney");
            string address = serviceRepo.GetServiceLocation("IAccountRepository");
            if (address == null || !address.Equals(accountRepositoryAddress)){
                if (address != null)
                {
                    accountRepositoryAddress = address;
                    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                    ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(binding, new EndpointAddress(accountRepositoryAddress));
                    IAccountRepository accountRepository = cf.CreateChannel();
                    transfer.AccountRepository = accountRepository;
                    log.InfoFormat("New IAccountRepository address: {0}",address);
                }
                else
                {
                    transfer.AccountRepository = null;
                    accountRepositoryAddress = null;
                }
            }            
        }
    }
}


