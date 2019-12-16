using System;
using Microsoft.EntityFrameworkCore;

namespace MiniDDD.UnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private  SqlClient<DbContext> _sqlClient;
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
    }
}
