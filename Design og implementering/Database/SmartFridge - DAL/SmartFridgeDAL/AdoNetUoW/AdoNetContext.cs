﻿using System.Collections.Generic;
using System.Data;
using System.Threading;
using SmartFridgeDAL.Connection;

namespace SmartFridgeDAL.AdoNetUoW
{
    public class AdoNetContext
    {
        private readonly IDbConnection _connection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<AdoNetUnitOfWork> _uows = new LinkedList<AdoNetUnitOfWork>();

        public AdoNetContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.Create();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var transaction = _connection.BeginTransaction();
            var uow = new AdoNetUnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

            _rwLock.EnterWriteLock();
            _uows.AddLast(uow);
            _rwLock.ExitWriteLock();
            return uow;
        }

        public IDbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();

            _rwLock.EnterReadLock();
            if (_uows.Count > 0)
                cmd.Transaction = _uows.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return cmd;
        }

        private void RemoveTransaction(AdoNetUnitOfWork obj)
        {
            _rwLock.EnterWriteLock();
            _uows.Remove(obj);
            _rwLock.ExitWriteLock();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
