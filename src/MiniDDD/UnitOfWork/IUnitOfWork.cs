using System;

namespace MiniDDD.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        T GetSqlClient<T>() where T : class;
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
