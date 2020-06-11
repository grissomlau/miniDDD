using System;
using Microsoft.EntityFrameworkCore;

namespace MiniDDD.UnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private SqlClient<DbContext> _sqlClient;
        private bool _disposed = false;
        public UnitOfWork(DefaultDbContext context)
        {
            _sqlClient = new SqlClient<DbContext>(context);
        }
        public void BeginTransaction()
        {
            _sqlClient.IsOpenedTransaction = true;
        }

        public void Commit()
        {
            _sqlClient.IsOpenedTransaction = false;
            _sqlClient.Client.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 手动释放托管资源
                }
                // 手动释放非托管资源

                if (_sqlClient?.Client != null)
                {
                    try
                    {
                        _sqlClient.Client.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _sqlClient = null;
                    }
                }

                _disposed = true;
            }
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(DefaultDbContext)))
            {
                throw new Exception($"cannot convert {typeof(T)} to {typeof(SqlClient<DefaultDbContext>)}");
            }
            return _sqlClient as SqlClient<T>;
        }

        public void Rollback()
        {
            _sqlClient.IsOpenedTransaction = false;
            //_sqlClient.Client.Database.BeginTransaction();
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
