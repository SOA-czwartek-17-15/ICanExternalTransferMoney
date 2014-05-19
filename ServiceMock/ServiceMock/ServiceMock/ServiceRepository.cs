using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.ServiceModel;

namespace ServiceMock
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ServiceRepository : IServiceRepository 
    {
        /*Zarejestrowanie Serwisu*/
        public void RegisterService(String Name, String Address)
        {
            Console.WriteLine("REGISTER: {0}({1})",Name,Address);
        }

        /*Pobranie adresu Serwisu*/
        public string GetServiceLocation(String Name)
        {
            if (Name.Equals("IAccountRepository"))
            {
                Console.WriteLine("GETSERVICELOCATION: {0}", Name);
                return "net.tcp://localhost:54321/AccountRepo";
            }
            else
            {
                Console.WriteLine("GETSERVICELOCATION: ERROR!!! Service: {0}",Name);
                return null;
            }
        }

        /*Wyrejestrowanie Serwisu*/
        public void Unregister(String Name)
        {
            Console.WriteLine("UNREGISTER: {0}",Name);
        }

        /*Zgłoszenie się, że Serwis nadal działa (po 5s od ostatniego zgłoszenia
         serwis uznany jest za niedziałający i usuwany)*/
        public void Alive(String Name)
        {
            Console.WriteLine("ALIVE: {0}",Name);
        }
    }
}
