namespace MiniUnitOfWork.Test
{
    public class NullUnitOfWork : IUnitOfWork
    {
        public void Dispose()
        {
        }

        public void SaveChanges()
        {
        }
    }
}