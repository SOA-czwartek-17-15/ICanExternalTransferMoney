using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ContractsAsync
{
    /// <summary>
    /// Interfejs przelewów i przychodów zewnętrznych asynchroniczny
    /// Mateusz Kotlarz, Kacper Półchłopek
    /// </summary>
    [ServiceContract]
    interface ICanExternalTransferMoneyAsync
    {
        /// <summary>
        /// Odbieranie pieniędzy zewnętrznych od klienta innego banku
        /// </summary>
        /// <param name="from">Numer konta klienta innego banku</param>
        /// <param name="to">Guid naszego klienta, który ma otrzymać środki</param>
        /// <param name="howMany">Kwota przelewu</param>
        /// <returns>Guid operacji, gdy pomyślnie lub Guid.Empty gdy błąd.</returns>
        [OperationContract]
        void ReceiveExternalMoney(string from, Guid to, double howMany, Guid ident, string serviceName);

        /// <summary>
        /// Wysyłanie pieniędzy do klienta w innym banku
        /// </summary>
        /// <param name="from">Guid naszego klienta</param>
        /// <param name="to">Numer konta klienta innego banku</param>
        /// <param name="howMany">Kwota przelewu</param>
        /// <returns>Guid operacji, gdy pomyślnie lub Guid.Empty gdy błąd.</returns>
        [OperationContract]
        void SendExternalMoney(Guid from, string to, double howMany, Guid ident, string serviceName);

        /// <summary>
        /// Sprawdz czy operacja o odpowienim idencie się skończyła
        /// </summary>
        /// <param name="ident"></param>
        /// <param name="serviceName"></param>
        [OperationContract]
        void CheckOperationStatus(Guid ident, string serviceName);
    }
}