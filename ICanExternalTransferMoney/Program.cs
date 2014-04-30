using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ProgamNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            //log4net
            string serviceRepository = "net.tcp://localhost:54321/ServiceRepo";//to powinno byc odczytane z app.config
            //strworzenie clienta do service repo
            //zarejestrowanie swojej uslugi
            //odpalenie timera w celu wysylania Alive
            //stworznie AccountRepository i przekazanie mu serviceRepository zeby odpytac o potrzebne sewisy

            // logowanie

            //odpytywanie servisów i tworzenie klientów

            //CanExternalTransferMoney transfer = new CanExternalTransferMoney(serviceRepository);
        }
    }

    public class CanExternalTransferMoney : Contracts.ICanExternalTransferMoney
    {
       // IAccountRepository acconts { set; }
        CanExternalTransferMoney()
        {
            //przyjmuje serviceRepo i szuka potrzebnych serwisów(AccountRepo)    
        }

        long ReceiveExternalMoney(Guid from, string to, double howMany)
        {
            //Sprawdza czy ma accountRepo, jeżeli nie to return -1
            //ustawiamy nową wartośc w acount.setBalance(from, howMany)
            //dodajemy log do bazy
            //printujemy log - console.write(przylano to howmany);
            //zwracamy nr operacji
            return -1;
        }

        long SendExternalMoney(string from, Guid to, double howMany)
        {
            //Sprawdza czy ma accountRepo, jeżeli nie to return -1
            //ustawiamy nową wartośc w acount.setBalance(from, howMany)
            //dodajemy log do bazy
            //printujemy log - console.write(przylano to howmany);
            //zwracamy nr operacji
            return -1;
        }
    }
}
