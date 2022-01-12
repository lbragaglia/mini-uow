using System;
using System.Data;

namespace MiniUnitOfWork.Test
{
    public class NullUnitOfWorkFactory : NullUnitOfWorkFactory<IUnitOfWork>
    {
        public NullUnitOfWorkFactory() : base(new NullUnitOfWork())
        {
        }
    }

    public class NullUnitOfWorkFactory<T> : IUnitOfWorkFactory<T> where T : IUnitOfWork
    {
        private readonly T _unitOfWork;

        public NullUnitOfWorkFactory(T unitOfWork) => _unitOfWork = unitOfWork;

        public T GetCurrent() => _unitOfWork;

        public T StartNew() => _unitOfWork;

        public T StartNew(IsolationLevel isolationLevel) => _unitOfWork;

        public TResult DoInUnitOfWork<TResult>(Func<TResult> func) => func();

        public void DoInUnitOfWork(Action action) => action();
    }
}