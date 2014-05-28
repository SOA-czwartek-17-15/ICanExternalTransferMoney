using ICanExternalTransferMoney.Domain;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICanExternalTransferMoney
{
    class NHibernateDAO : DAO
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NHibernateDAO));

        public ISession Session{set; get;}

        public NHibernateDAO()
        {
            NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
            config.Configure();
            config.AddAssembly(typeof(Potwierdzenie).Assembly);
            new SchemaExport(config).Execute(false, false, false); //Drugi na true gdy chcemy dropTable robić przy każdym uruchomieniu, false gdy mamy już uworzoną tabele
            ISessionFactory factory = config.BuildSessionFactory();
            Session = factory.OpenSession();
        }

        /// <summary>
        /// Zapisz potwierdzenie w bazie
        /// </summary>
        /// <param name="potwierdzenie">Potiwerdzenie class</param>
        /// <returns>True - gdy zapisano, false - w razie błędu</returns>
        public bool SavePotwierdzenieToBase(Potwierdzenie potwierdzenie)
        {
            ITransaction transaction = Session.BeginTransaction();
            Session.Save(potwierdzenie);
            transaction.Commit();
            return true;
        }

        ~NHibernateDAO()
        {
            Session.Close();
        }
    }
}
