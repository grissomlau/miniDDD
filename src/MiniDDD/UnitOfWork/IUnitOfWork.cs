using System;

namespace MiniDDD.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        SqlClient<T> GetSqlClient<T>() where T : class;

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
