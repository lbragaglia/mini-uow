using System.Data;

namespace MiniUnitOfWork
{
    public interface IUnitOfWorkFactory<out T> where T : IUnitOfWork
    {
        T GetCurrent();
        T StartNew();
        T StartNew(IsolationLevel isolationLevel);
    }
}