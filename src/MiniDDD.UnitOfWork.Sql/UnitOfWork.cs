using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Data;

namespace MiniDDD.UnitOfWork.Sql
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly DbContextOptions _options;
        private DbTransaction _dbTransaction;
        private bool _disposed = false;

        private DbConnection _cnn;
        public T GetSqlClient<T>() where T : class
        {
            if (_cnn == null)
            {
                if (string.IsNullOrEmpty(_options.ProviderName))
                {
                    throw new MiniDDDException("DbContextOptions.ProviderName cannot be null or empty, please specify the SqlProvider");
                }
                DbProviderFactory factory = DbProviderFactories.GetFactory(_options.ProviderName);
                _cnn = factory.CreateConnection();
                _cnn.ConnectionString = _options.ConnectionString;

            }
            if (!typeof(T).IsAssignableFrom(typeof(DbConnection)))
            {
                throw new InvalidCastException($"cannot convert {typeof(T)} to DbConnection");
            }
            return _cnn as T;
        }
        public UnitOfWork(DbContextOptions options)
        {
            _options = options;
        }

        public void BeginTransaction()
        {
            if (_cnn.State != System.Data.ConnectionState.Open)
            {
                _cnn.Open();
            }
            _dbTransaction = _cnn.BeginTransaction();
        }

        public void Commit()
        {
            _dbTransaction.Commit();
        }

        public void Rollback()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
            }
            //_sqlClient.IsOpenedTransaction = false;
            Dispose();

        }

        public void Dispose()
        {
            Dispose(true);//同时释放托管和非托管资源
                          // GC.SuppressFinalize(this);//阻止GC调用析构函数
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

                if (_cnn != null)
                {
                    try
                    {
                        _cnn.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _cnn = null;
                    }
                }

                _disposed = true;
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_cnn.State != System.Data.ConnectionState.Open)
            {
                _cnn.Open();
            }
            _dbTransaction = _cnn.BeginTransaction(isolationLevel);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
