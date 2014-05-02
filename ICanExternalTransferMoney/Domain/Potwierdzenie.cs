using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICanExternalTransferMoney.Domain
{
    public class Potwierdzenie
    {
        protected Potwierdzenie() { }
        public Potwierdzenie(string _TypOperacji, string _NrNadawcy, string _NrOdbiorcy, double _Kwota)
        {
            IdPotwierdzenia = Guid.NewGuid();
            Console.WriteLine(IdPotwierdzenia);
            TypOperacji = _TypOperacji;
            NrNadawcy = _NrNadawcy;
            NrOdbiorcy = _NrOdbiorcy;
            Kwota = _Kwota;
            DataOperacji = DateTime.Now;
        }

        public virtual Guid IdPotwierdzenia { set; get; }
        public virtual string TypOperacji { set; get; }
        public virtual string NrNadawcy { set; get; }
        public virtual string NrOdbiorcy { set; get; }
        public virtual double Kwota { set; get; }
        public virtual DateTime DataOperacji { set; get; }
    }
}
