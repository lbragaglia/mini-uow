using System;
using System.Data;
using MiniUnitOfWork.Spi;

namespace MiniUnitOfWork
{
    public class DbUnitOfWork : IUnitOfWork
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly IUnitOfWorkCloseable _session;

        public DbUnitOfWork(IDbConnection connection, IsolationLevel isolationLevel, IUnitOfWorkCloseable session)
        {
            _connection = connection;
            _session = session;
            _transaction = _connection.BeginTransaction(isolationLevel);
        }

        public void SaveChanges() => _transaction.Commit();

        public T DoInTransaction<T>(Func<IDbConnection, IDbTransaction, T> func) => func(_connection, _transaction);

        public void DoInTransaction(Action<IDbConnection, IDbTransaction> action) => action(_connection, _transaction);

        public void Dispose()
        {
            try
            {
                _transaction.Dispose();
            }
            finally
            {
                _session.Finish();
            }
        }
    }
}