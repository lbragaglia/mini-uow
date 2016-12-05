using System;
using System.Data;
using System.Data.SqlClient;


namespace yamuh
{
    public class DbContext
    {
        private readonly string _connectionString;
        private readonly IsolationLevel _level;

        public DbContext(string connectionString, IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            _connectionString = connectionString;
            _level = level;
        }

        public T TransactionExecute<T>(Func<IDbConnection, IDbTransaction, T> action)
        {
            return Execute(c =>
            {
                using (var tx = c.BeginTransaction(_level))
                {
                    T result = action.Invoke(c, tx);
                    tx.Commit();
                    return result;
                }
            });
        }

        public void TransactionExecute(Action<IDbConnection, IDbTransaction> action)
        {
            TransactionExecute<object>((c, tx) => { action.Invoke(c, tx); return null; });
        }

        public void Execute(Action<IDbConnection> action)
        {
            Execute<object>(c => { action.Invoke(c); return null; });
        }

        public T Execute<T>(Func<IDbConnection, T> action)
        {
            using (var c = new SqlConnection(_connectionString))
            {
                c.Open();
                return action.Invoke(c);
            }
        }
    }
}