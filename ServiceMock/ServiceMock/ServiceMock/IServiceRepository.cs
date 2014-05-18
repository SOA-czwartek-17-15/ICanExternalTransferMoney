using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Contracts
{
    [ServiceContract]
    public interface IServiceRepository
    {
        /*Zarejestrowanie Serwisu*/
        [OperationContract]
        void RegisterService(String Name, String Address);

        /*Pobranie adresu Serwisu*/
        [OperationContract]
        string GetServiceLocation(String Name);

        /*Wyrejestrowanie Serwisu*/
        [OperationContract]
        void Unregister(String Name);

        /*Zgłoszenie się, że Serwis nadal działa (po 5s od ostatniego zgłoszenia
         serwis uznany jest za niedziałający i usuwany)*/
        [OperationContract]
        void Alive(String Name);
    }
}