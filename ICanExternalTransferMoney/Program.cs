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
using System.IO;
using log4net.Config;

namespace ICanExternalTransferMoney
{
    class Program
    {
        //TODO logowanie - log4net

        private IServiceRepository serviceRepo;
        private CanExternalTransferMoney transfer;
        private string accountRepositoryAddress = null;

        static void Main(string[] args) { new Program(); }

        public Program() 
        {
            //log4net kod



            //Wyciąganie adresu ServiceRepository z App.config i uzyskanie ServiceRepo
            string serviceRepositoryAddress = ConfigurationManager.AppSettings["serviceRepositoryAddress"];
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            ChannelFactory<IServiceRepository> cf = new ChannelFactory<IServiceRepository>(binding, new EndpointAddress(serviceRepositoryAddress));
            serviceRepo = cf.CreateChannel();

            //---------log----------
            LogHelper.GetLog().Info("Got App.config adress and servicerepo");
            //---------log----------


            //Otwarcie sesji NHibernate
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();
            config.AddAssembly(typeof(Potwierdzenie).Assembly);
            new SchemaExport(config).Execute(false, false, false); //Drugi na true gdy chcemy dropTable robić przy każdym uruchomieniu, false gdy mamy już uworzoną tabele
            ISessionFactory factory = config.BuildSessionFactory();
            ISession session = factory.OpenSession();
            ITransaction transaction = session.BeginTransaction();

            //---------log----------
            LogHelper.GetLog().Info("NHibernate is opened");
            //---------log----------

            //Utworzenie Serwisu ICanExternalTransferMoney
            transfer = new CanExternalTransferMoney(session);
            string serviceAdress = ConfigurationManager.AppSettings["serviceAddress"];
            var sh = new ServiceHost(transfer, new Uri[] { new Uri(serviceAdress) });
            NetTcpBinding bindingOUT = new NetTcpBinding(SecurityMode.None);
            sh.AddServiceEndpoint(typeof(Contracts.ICanExternalTransferMoney), bindingOUT, serviceAdress);
            sh.Open();

            //---------log----------
            LogHelper.GetLog().Info("Service has been made");
            //---------log----------

            //Rejestracja Serwisu w ServiceRepository i odpalenie timera
            serviceRepo.RegisterService("ICanExternalTransferMoney", serviceAdress);
            Timer timer = new Timer();
            timer.Interval = Int16.Parse(ConfigurationManager.AppSettings["aliveSignalDelay"]);
            timer.Elapsed += new ElapsedEventHandler(TimerOnTick);
            timer.Start();

            //---------log----------
            LogHelper.GetLog().Info("Service has been registered, timer is up");
            //---------log----------



            //Pewnie pójdzie do usunięcie WriteLine jak będą logi, ReadLine potrzebny
            Console.WriteLine("Service Repo: {0}", serviceRepositoryAddress);
            Console.WriteLine("Kliknij Enter, aby wyłączyć serwis...");
            Console.ReadLine();

            //Zamknięcie sesji nhibernate
            session.Close();
            timer.Stop();

            //---------log----------
            LogHelper.GetLog().Info("NHibernate is not up anymore");
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
            serviceRepo.IsAlive("ICanExternalTransferMoney");
            string address = serviceRepo.GetServiceAddress("IAccountRepository");
            if (address == null || !address.Equals(accountRepositoryAddress)){
                if (address != null)
                {
                    accountRepositoryAddress = address;
                    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                    ChannelFactory<IAccountRepository> cf = new ChannelFactory<IAccountRepository>(binding, new EndpointAddress(accountRepositoryAddress));
                    IAccountRepository accountRepository = cf.CreateChannel();
                    transfer.AccountRepository = accountRepository;
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

public class LogHelper
{
static LogHelper()
{
var confFile = ConfigurationSettings.AppSettings.Get("log4net.config");
var fi = new FileInfo(confFile);
XmlConfigurator.Configure(fi);
}

public static ILog GetLog()
{
return LogManager.GetLogger("WebAppLog");
}
}


