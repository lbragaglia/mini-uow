namespace MiniUnitOfWork.Spi
{
    public interface IUnitOfWorkCloseable
    {
        void Finish();
    }
}