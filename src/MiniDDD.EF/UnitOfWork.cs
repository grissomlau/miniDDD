using System;
using Microsoft.EntityFrameworkCore;

namespace MiniDDD.UnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlClient<DbContext> _sqlClient;
        public UnitOfWork(DefaultDbContext context)
        {
            _sqlClient = new SqlClient<DbContext>(context);
        }
        public void BeginTran()
        {
            _sqlClient.IsBeginTran = true;
        }

        public void CommitTran()
        {
            _sqlClient.IsBeginTran = false;
            _sqlClient.Client.SaveChanges();
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(DefaultDbContext)))
            {
                throw new Exception($"cannot convert {typeof(T)} to {typeof(SqlClient<DefaultDbContext>)}");
            }
            return _sqlClient as SqlClient<T>;
        }

        public void RollbackTran()
        {
            _sqlClient.IsBeginTran = false;
            //_sqlClient.Client.Database.BeginTransaction();
        }
    }
}
