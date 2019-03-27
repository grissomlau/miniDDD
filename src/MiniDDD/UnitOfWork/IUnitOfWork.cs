namespace MiniDDD.UnitOfWork
{
    public interface IUnitOfWork
    {
        SqlClient<T> GetSqlClient<T>() where T : class;

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
