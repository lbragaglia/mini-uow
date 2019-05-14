using System;
using System.Data;
using System.Threading;
using MiniUnitOfWork.Spi;

namespace MiniUnitOfWork
{
    public class DbUnitOfWorkFactory : IUnitOfWorkFactory<DbUnitOfWork>, IUnitOfWorkCloseable, IDisposable
    {
        private readonly ThreadLocal<DbUnitOfWork> _current = new ThreadLocal<DbUnitOfWork>();
        private readonly IDbConnection _connection;
        private readonly IsolationLevel _defaultIsolationLevel;

        public DbUnitOfWorkFactory(IDbConnectionFactory connectionFactory,
            IsolationLevel defaultIsolationLevel = IsolationLevel.ReadCommitted)
        {
            _connection = connectionFactory.NewConnection();
            _connection.Open();
            _defaultIsolationLevel = defaultIsolationLevel;
        }

        public DbUnitOfWork GetCurrent() => _current.Value;

        public DbUnitOfWork StartNew() => StartNew(_defaultIsolationLevel);

        public DbUnitOfWork StartNew(IsolationLevel isolationLevel)
        {
            if (_current.IsValueCreated && _current.Value != null)
                throw new InvalidOperationException("Pending tx in progress");
            return _current.Value = new DbUnitOfWork(_connection, isolationLevel, this);
        }

        public void Finish() => _current.Value = null;

        public void Dispose()
        {
            try
            {
                _current?.Dispose();
            }
            finally
            {
                _connection?.Dispose();
            }
        }
    }
}