using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using Dapper;
using Npgsql;
using System.Collections.Generic;

namespace MiniDDD.UnitOfWork.Dapper
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
                switch (_options.DbType)
                {
                    case DbType.MySQL:
                        _cnn = new MySqlConnection(_options.ConnectionString);
                        break;
                    case DbType.SQLServer:
                        _cnn = new SqlConnection(_options.ConnectionString);
                        break;
                    case DbType.Oracle:
                        _cnn = new OracleConnection(_options.ConnectionString);
                        break;
                    case DbType.PostgreSQL:
                        _cnn = new NpgsqlConnection(_options.ConnectionString);
                        break;
                    case DbType.SQLite:
                        _cnn = new SQLiteConnection(_options.ConnectionString);
                        break;
                    default:
                        throw new Exception("please specify DbType!");
                }
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
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
