using System;
using System.Data;
using System.Data.SqlClient;

namespace miniuow
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly IsolationLevel _level;

        public DbHelper(string connectionString) : this(connectionString, IsolationLevel.ReadCommitted)
        {
        }

        public DbHelper(string connectionString, IsolationLevel level)
        {
            _connectionString = connectionString;
            _level = level;
        }

        public T TransactionExecute<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            return Execute(connection =>
            {
                using (var transaction = connection.BeginTransaction(_level))
                {
                    var result = action.Invoke(connection, transaction);
                    transaction.Commit();
                    return result;
                }
            });
        }

        public void TransactionExecute(Action<IDbConnection, IDbTransaction> action)
        {
            TransactionExecute<object>((connection, transaction) =>
            {
                action.Invoke(connection, transaction);
                return null;
            });
        }

        public void Execute(Action<IDbConnection> action)
        {
            Execute<object>(connection =>
            {
                action.Invoke(connection);
                return null;
            });
        }

        public T Execute<T>(Func<IDbConnection, T> action)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return action.Invoke(connection);
            }
        }
    }
}