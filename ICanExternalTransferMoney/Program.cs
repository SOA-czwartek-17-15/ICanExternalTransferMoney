using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using ICanExternalTransferMoney.Domain;
using Contracts;
using System.Timers;
using log4net;
using log4net.Config;
using System.IO;

namespace ICanExternalTransferMoney
{
    class Program
    {
        //private IServiceRepository serviceRepo;
        private CanExternalTransferMoney transfer;
        private string accountRepositoryAddress = null;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        //private ChannelFactory<IServiceRepository> servCF;
        private ServiceRepoZeroMQClient serviceZMQClient;
        private string serviceAdress = ConfigurationManager.AppSettings["serviceAddress"];

        private bool registerdOnServiceRepo = false;

        static void Main(string[] args)
        {
            //try { 
                new Program(); //}
            //catch (Exception ex)
            //{
            //    log.Error(ex.Message);
            //    Console.WriteLine(ex.Message);
            //    Console.ReadLine();
            //}
        }

        public Program()
        {
            //Stworzenie DAO
            DAO dao = new NHibernateDAO();

            //---------log----------
            Console.WriteLine("DAO is created and avaible");
            log.Info("DAO is created and avaible");
            //---------log----------

            transfer = new CanExternalTransferMoney(dao);
            //---------log----------
            Console.WriteLine("Service has been made");
            log.Info("Service has been made");
            //---------log----------

            //Uruchomienie wątku ZeroMQ
            ZeroMQServer zeroMQServer = new ZeroMQServer("tcp://127.0.0.1:5577", transfer);
            System.Threading.Thread zeroMQServerThread = new System.Threading.Thread(new System.Threading.ThreadStart(zeroMQServer.Receive));
            zeroMQServerThread.Start();

            //Wyciąganie adresu ServiceRepository z App.config i uzyskanie ServiceRepo
            string serviceRepositoryAddress = ConfigurationManager.AppSettings["serviceRepositoryAddress"];
            serviceZMQClient = new ServiceRepoZeroMQClient(serviceRepositoryAddress);

            //timer
            Timer timer = new Timer();
            timer.Interval = Double.Parse(ConfigurationManager.AppSettings["aliveSignalDelay"]);
            timer.Elapsed += new ElapsedEventHandler(TimerOnTick);
            timer.Start();
            //TimerOnTick(null, null);

            //---------log----------
            Console.WriteLine("Timer started");
            log.Info("Timer started");
            //---------log----------

            //ReadLine kończący program
            Console.WriteLine("ICanExternalTransferMoney is on!");
            Console.WriteLine("ICanExternalTransferMoneyAsync is on!");
            Console.WriteLine("Kliknij Enter, aby wyłączyć serwis...");
            Console.ReadLine();

            timer.Stop();
            //---------log----------
            Console.WriteLine("Timer stopped");
            log.Info("Timer stopped");
            //---------log----------

            if (registerdOnServiceRepo)
            {
                if (serviceZMQClient.Unregister())
                {
                    //---------log----------
                    Console.WriteLine("Unregistered in IServiceRepository");
                    log.Info("Unregistered in IServiceRepository");
                    //---------log----------
                }
                else
                {
                    //---------log----------
                    Console.WriteLine("Unregister failed, because Service is dead.");
                    log.Info("Unregister failed, because Service is dead.");
                    //---------log----------
                }
            }

            //---------log----------
            Console.WriteLine("Service Endpoint Closed.");
            log.Info("Service Endpoint Closed.");
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
            try
            {
                //Rejestracja Serwisu w ServiceRepository i odpalenie timera
                if (!registerdOnServiceRepo)
                {
                    if (serviceZMQClient.Register(serviceAdress))
                    {
                        registerdOnServiceRepo = true;

                        //---------log----------
                        Console.WriteLine("Service has been registered");
                        log.Info("Service has been registered");
                        //---------log----------
                    }
                    else throw new EndpointNotFoundException();
                }

                try
                {
                    if (serviceZMQClient.Alive()) Console.Write(".");
                    else throw new EndpointNotFoundException();

                    string address = serviceZMQClient.GetServiceLocation("IAccountService");
                    if (address == null || !address.Equals(accountRepositoryAddress))
                    {
                        if (address != null)
                        {
                            accountRepositoryAddress = address;
                            transfer.AccountZMQClient = new AccountRepoZeroMQClient(accountRepositoryAddress);

                            //---------log----------
                            log.InfoFormat("New IAccountRepository address: {0}", address);
                            Console.WriteLine("New IAccountRepository address: {0}", address);
                            //---------log----------
                        }
                        else
                        {
                            transfer.AccountZMQClient = null;
                            accountRepositoryAddress = null;

                            //---------log----------
                            log.InfoFormat("AccountRepo not registered on ServiceRepo");
                            Console.WriteLine("AccountRepo not registered on ServiceRepo");
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
            catch (EndpointNotFoundException ex)
            {
                //---------log----------
                Console.WriteLine("ServiceRepository not found. Retry after 3s.");
                log.Error("ServiceRepository not found. Retry after 3s.");
                //---------log----------
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Error(ex.Message);
            }
        }
    }
}


