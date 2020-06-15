using System;
using System.Data;

namespace MiniDDD.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        T GetSqlClient<T>() where T : class;
        void BeginTransaction(IsolationLevel isolationLevel);
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
