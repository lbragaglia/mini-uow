namespace MiniUnitOfWork
{
    public static class SqlUnitOfWorkFactory
    {
        public static DbUnitOfWorkFactory For(string connectionString) =>
            new DbUnitOfWorkFactory(new SqlConnectionFactory(connectionString));
    }
}