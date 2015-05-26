using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using DataAccessLayer.Connection;

namespace DataAccessLayer.AdoNetUoW
{
    /// <summary>
    /// ADO.net Context class.
    /// </summary>
    public class AdoNetContext : IContext
    {
        private readonly IDbConnection _connection;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<AdoNetUnitOfWork> _uows = new LinkedList<AdoNetUnitOfWork>();

        /// <summary>
        /// Injects a database connection and opens it.
        /// </summary>
        /// <param name="connectionFactory"></param>
        public AdoNetContext(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.Create();
        }

        /// <summary>
        /// Creates a AdoNetUnitOfWork object for database transactions. Can be used for using scopes for transactions.
        /// </summary>
        /// <returns>IUnitOfWork interface.</returns>
        public IUnitOfWork CreateUnitOfWork()
        {
            var transaction = _connection.BeginTransaction();
            var uow = new AdoNetUnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

            _rwLock.EnterWriteLock();
            _uows.AddLast(uow);
            _rwLock.ExitWriteLock();
            return uow;
        }

        /// <summary>
        /// Creates a command to be executed. Can be used for using scopes with database commands. 
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();

            _rwLock.EnterReadLock();
            if (_uows.Count > 0)
                cmd.Transaction = _uows.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return cmd;
        }

        /// <summary>
        /// Removes previous transaction.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveTransaction(AdoNetUnitOfWork obj)
        {
            _rwLock.EnterWriteLock();
            _uows.Remove(obj);
            _rwLock.ExitWriteLock();
        }

        /// <summary>
        /// Dispo
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
