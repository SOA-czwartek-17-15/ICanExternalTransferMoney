using System;
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
            var sh2 = new ServiceHost(ServiceRepo, new Uri[] { new Uri(serviceAdress) });
            NetTcpBinding bindingOUT2 = new NetTcpBinding(SecurityMode.None);
            sh2.AddServiceEndpoint(typeof(Contracts.IServiceRepository), bindingOUT2, serviceAdress2);
            sh2.Open();

            Console.ReadLine();
        }
    }
}
