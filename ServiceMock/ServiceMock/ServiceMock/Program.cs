﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMock
{
    class Program
    {
        static void Main(string[] args) { new Program();  }
        private ServiceRepository ServiceRepo;
        private AccountRepository AccountRepo;

        public Program()
        {
            ServiceRepo = new ServiceRepository();
            string serviceAdress = "net.tcp://localhost:54321/ServiceRepo";
            var sh = new ServiceHost(ServiceRepo, new Uri[] { new Uri(serviceAdress) });
            NetTcpBinding bindingOUT = new NetTcpBinding(SecurityMode.None);
            sh.AddServiceEndpoint(typeof(Contracts.IServiceRepository), bindingOUT, serviceAdress);
            sh.Open();

            AccountRepo = new AccountRepository();
            string serviceAdress2 = "net.tcp://localhost:54321/AccountRepo";
            var sh2 = new ServiceHost(AccountRepo, new Uri[] { new Uri(serviceAdress2) });
            NetTcpBinding bindingOUT2 = new NetTcpBinding(SecurityMode.None);
            sh2.AddServiceEndpoint(typeof(Contracts.IAccountRepository), bindingOUT2, serviceAdress2);
            sh2.Open();

            string tekst = Console.ReadLine();

            while (tekst.Equals("n") || tekst.Equals("a"))
            {
                if(tekst.Equals("n"))
                {
                    Contracts.ICanExternalTransferMoney transfer;
                    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                    ChannelFactory<Contracts.ICanExternalTransferMoney> cf = new ChannelFactory<Contracts.ICanExternalTransferMoney>(binding, new EndpointAddress("net.tcp://localhost:50008/ICanExternalTransferMoney"));
                    transfer = cf.CreateChannel();

                    Guid wysylka = transfer.SendExternalMoney(Guid.NewGuid(), "abc", 12.5);
                    Guid odbior = transfer.ReceiveExternalMoney("bcd", Guid.NewGuid() , 32.4);

                    Console.WriteLine(wysylka);
                    Console.WriteLine(odbior);
                }
                else if(tekst.Equals("a"))
                {
                    ContractsAsync.ICanExternalTransferMoneyAsync transferAsync;
                    NetTcpBinding bindingAsync = new NetTcpBinding(SecurityMode.None);
                    ChannelFactory<ContractsAsync.ICanExternalTransferMoneyAsync> cfAsync = new ChannelFactory<ContractsAsync.ICanExternalTransferMoneyAsync>(bindingAsync, new EndpointAddress("net.tcp://localhost:50008/ICanExternalTransferMoneyAsync"));
                    transferAsync = cfAsync.CreateChannel();

                    transferAsync.ReceiveExternalMoney("Kowalski", Guid.Empty, 20, Guid.NewGuid(), "MOCK");
                    transferAsync.SendExternalMoney(Guid.Empty, "Nowak", 30, Guid.NewGuid(), "MOCK");
                    transferAsync.CheckOperationStatus(Guid.NewGuid(), "MOCK");
                }

                tekst = Console.ReadLine();
            }

        }
    }
}
