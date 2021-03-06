﻿using System;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EasyUnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;

        private DefaultDbContext _dbContext;
        public T GetUowWorker<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(DefaultDbContext)))
            {
                throw new UnitOfWorkException($"cannot convert {typeof(T)} to {typeof(DefaultDbContext)}");
            }
            return _dbContext as T;
        }

        public UnitOfWork(DefaultDbContext context)
        {
            _dbContext = context;
        }
        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            _dbContext.Database.BeginTransaction(isolationLevel);
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
            _dbContext.Database.CommitTransaction();
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

                //-- DbContext 不需要手动释放，在调用 SaveChange 自动释放
                //if (_dbContext != null)
                //{
                //    try
                //    {
                //        _dbContext.Dispose();
                //    }
                //    catch
                //    {
                //    }
                //    finally
                //    {
                //        _dbContext = null;
                //    }
                //}

                _disposed = true;
            }
        }

        public void Rollback()
        {
            _dbContext.Database.RollbackTransaction();
        }



        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
