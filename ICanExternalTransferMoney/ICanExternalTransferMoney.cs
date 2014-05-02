using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Contracts
{
    /// <summary>
    /// Interfejs przelewów i przychodów zewnętrznych.
    /// Mateusz Kotlarz, Kacper Półchłopek
    /// </summary>
    [ServiceContract]
    public interface ICanExternalTransferMoney
    {
        /// <summary>
        /// Odbieranie pieniędzy zewnętrznych od klienta innego banku
        /// </summary>
        /// <param name="from">Numer konta klienta innego banku</param>
        /// <param name="to">Guid naszego klienta, który ma otrzymać środki</param>
        /// <param name="howMany">Kwota przelewu</param>
        /// <returns>Guid operacji, gdy pomyślnie lub Guid.Empty gdy błąd.</returns>
        [OperationContract]
        Guid ReceiveExternalMoney(string from, Guid to, double howMany);

        /// <summary>
        /// Wysyłanie pieniędzy do klienta w innym banku
        /// </summary>
        /// <param name="from">Guid naszego klienta</param>
        /// <param name="to">Numer konta klienta innego banku</param>
        /// <param name="howMany">Kwota przelewu</param>
        /// <returns>Guid operacji, gdy pomyślnie lub Guid.Empty gdy błąd.</returns>
        [OperationContract]
        Guid SendExternalMoney(Guid from, string to, double howMany);
    }
}