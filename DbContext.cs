using System;
using System.Data;
using System.Data.SqlClient;


namespace yamuh
{
    public class DbContext : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly IsolationLevel _level;

        public DbContext(string connectionString) : this(connectionString, IsolationLevel.ReadCommitted)
        {
        }

        public DbContext(string connectionString, IsolationLevel level)
        {
            _connection = new SqlConnection(connectionString);
            _level = level;
        }

        public UnitOfWork CreateUnitOfWork()
        {
            return CreateUnitOfWork(_level);
        }

        public UnitOfWork CreateUnitOfWork(IsolationLevel level)
        {
            return new UnitOfWork(_connection, level);
        }

        public void Open()
        {
            _connection.Open();
        }

        public void WithConnection(Action<IDbConnection> action)
        {
            WithConnection<object>(connection =>
            {
                action.Invoke(connection);
                return null;
            });
        }

        public T WithConnection<T>(Func<IDbConnection, T> action)
        {
            return action.Invoke(_connection);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}