using System;
using System.Data;

namespace EasyUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        T GetUowWorker<T>() where T : class;
        void BeginTransaction(IsolationLevel isolationLevel);
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
