using System;
using System.Data;

namespace DataAccessLayer.AdoNetUoW
{
    /// <summary>
    /// Unit of Work class for database transactions.
    /// </summary>
    public class AdoNetUnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction;
        private readonly Action<AdoNetUnitOfWork> _rolledBack;
        private readonly Action<AdoNetUnitOfWork> _committed;

        /// <summary>
        /// Injects a transactions, and transaction actions.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="rolledBack"></param>
        /// <param name="committed"></param>
        public AdoNetUnitOfWork(IDbTransaction transaction, Action<AdoNetUnitOfWork> rolledBack, Action<AdoNetUnitOfWork> committed)
        {
            Transaction = transaction;
            _transaction = transaction;
            _rolledBack = rolledBack;
            _committed = committed;
        }

        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Rollbacks and disposes the transaction.
        /// </summary>
        public void Dispose()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _rolledBack(this);
            _transaction = null;
        }

        /// <summary>
        /// Commit the database transactions.
        /// </summary>
        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Don't call save changes twice.");

            _transaction.Commit();
            _committed(this);
            _transaction = null;
        }
    }
}
