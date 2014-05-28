using ICanExternalTransferMoney.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICanExternalTransferMoney
{
    public interface DAO
    {
        /// <summary>
        /// Zapisz potwierdzenie w bazie
        /// </summary>
        /// <param name="potwierdzenie">Potiwerdzenie class</param>
        /// <returns>True - gdy zapisano, false - w razie błędu</returns>
        bool SavePotwierdzenieToBase(Potwierdzenie potwierdzenie);
    }
}
