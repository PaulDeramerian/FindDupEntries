using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DuplicateAdvisorMatch.DAL
{
    public interface IUnitOfWork : IDisposable
    {

        /// <summary>
        /// Call this to commit the unit of work
        /// </summary>
        void Commit();

        /// <summary>
        /// Return the database reference for this UOW
        /// </summary>
        adContext Db { get; }

        /// <summary>
        /// Starts a transaction on this unit of work
        /// </summary>
        void StartTransaction();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;

        private System.Transactions.TransactionScope _transaction;
        private readonly adContext _db;
        
        public UnitOfWork()
        {
            if (ConfigurationManager.ConnectionStrings["AdvertiserDB"] == null)
                throw new Exception("DB connection missing");

            _connectionString = ConfigurationManager.ConnectionStrings["AdvertiserDB"].ConnectionString;
            _db = new adContext(_connectionString);
        }

        public void Dispose()
        {

        }

        public void StartTransaction()
        {
            _transaction = new TransactionScope();
        }

        public void Commit()
        {
            _db.SaveChanges();
            _transaction.Complete();
        }

        public adContext Db
        {
            get { return _db; }
        }


    }
}
