using System;
using System.Data;
using System.Data.SqlClient;

namespace yamuh
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlTransaction _transaction;

        public UnitOfWork(SqlConnection connection, IsolationLevel level)
        {
            _transaction = connection.BeginTransaction(level);
        }

        public void SaveChanges()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}