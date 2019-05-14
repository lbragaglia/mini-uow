using System;
using System.Data;
using MiniUnitOfWork.Spi;
using Xunit;
using Moq;

namespace MiniUnitOfWork.Tests
{
    public class DbUnitOfWorkTests
    {
        private readonly Mock<IDbConnectionFactory> _connectionFactory;
        private readonly Mock<IDbConnection> _connection;
        private readonly Mock<IDbTransaction> _transaction;
        private readonly MockRepository _mockRepository;

        public DbUnitOfWorkTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            _connectionFactory = _mockRepository.Create<IDbConnectionFactory>();
            _connection = _mockRepository.Create<IDbConnection>();
            _transaction = _mockRepository.Create<IDbTransaction>();
        }

        [Fact]
        public void FactoryKeepsCurrentUow()
        {
            var factory = GivenAUnitOfWorkFactory();
            var uow = GivenAUnitOfWork(factory);

            Assert.Same(uow, factory.GetCurrent());

            DisposeUnitOfWork(uow);
            Assert.Null(factory.GetCurrent());
            DisposeUnitOfWorkFactory(factory);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CannotStartANewUowUntilCurrentOneFinish()
        {
            var factory = GivenAUnitOfWorkFactory();
            var firstUow = GivenAUnitOfWork(factory);

            Assert.Throws<InvalidOperationException>(() => factory.StartNew());

            DisposeUnitOfWork(firstUow);
            var secondUow = GivenAUnitOfWork(factory);

            Assert.NotSame(firstUow, secondUow);

            DisposeUnitOfWork(secondUow);
            DisposeUnitOfWorkFactory(factory);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void CommitUnitOfWork()
        {
            ExpectCommittingUnitOfWork();
            ExpectUnitOfWorkDisposal();

            var factory = GivenAUnitOfWorkFactory();
            using (var uow = GivenAUnitOfWork(factory))
            {
                DoCallbackInTransaction(uow);

                uow.SaveChanges();
            }

            DisposeUnitOfWorkFactory(factory);
            _mockRepository.VerifyAll();
        }

        private void DoCallbackInTransaction(DbUnitOfWork uow)
        {
            var expectedResult = new object();
            var actualResult = uow.DoInTransaction((conn, tx) =>
            {
                Assert.Same(_connection.Object, conn);
                Assert.Same(_transaction.Object, tx);
                return expectedResult;
            });
            Assert.Same(expectedResult, actualResult);
        }

        private void ExpectCommittingUnitOfWork() => _transaction.Setup(t => t.Commit());

        private DbUnitOfWorkFactory GivenAUnitOfWorkFactory()
        {
            ExpectOpeningAConnection();
            return new DbUnitOfWorkFactory(_connectionFactory.Object);
        }

        private void ExpectOpeningAConnection()
        {
            _connectionFactory.Setup(cf => cf.NewConnection()).Returns(_connection.Object);
            _connection.Setup(c => c.Open());
        }

        private DbUnitOfWork GivenAUnitOfWork(DbUnitOfWorkFactory uowFactory)
        {
            const IsolationLevel anIsolationLevel = IsolationLevel.RepeatableRead;
            ExpectUnitOfWorkCreation(anIsolationLevel);
            return uowFactory.StartNew(anIsolationLevel);
        }

        private void ExpectUnitOfWorkCreation(IsolationLevel isolationLevel)
        {
            _connection.Setup(c => c.BeginTransaction(isolationLevel)).Returns(_transaction.Object);
        }

        private void DisposeUnitOfWork(IDisposable uow)
        {
            ExpectUnitOfWorkDisposal();
            uow.Dispose();
        }

        private void ExpectUnitOfWorkDisposal()
        {
            _transaction.Setup(t => t.Dispose());
        }

        private void DisposeUnitOfWorkFactory(IDisposable uowFactory)
        {
            ExpectUnitOfWorkFactoryDisposal();
            uowFactory.Dispose();
        }

        private void ExpectUnitOfWorkFactoryDisposal()
        {
            _connection.Setup(c => c.Dispose());
        }
    }
}