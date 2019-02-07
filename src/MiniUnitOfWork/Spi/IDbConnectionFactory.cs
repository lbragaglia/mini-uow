using System.Data;

namespace MiniUnitOfWork.Spi
{
    public interface IDbConnectionFactory
    {
        IDbConnection NewConnection();
    }
}