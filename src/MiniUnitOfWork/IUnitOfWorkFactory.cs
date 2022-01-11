using System;
using System.Data;

namespace MiniUnitOfWork
{
    public interface IUnitOfWorkFactory<out T> where T : IUnitOfWork
    {
        T GetCurrent();
        T StartNew();
        T StartNew(IsolationLevel isolationLevel);
        TResult DoInUnitOfWork<TResult>(Func<TResult> func);
        void DoInUnitOfWork(Action action);
    }
}