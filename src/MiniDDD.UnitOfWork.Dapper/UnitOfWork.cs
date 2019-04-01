using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using Dapper;
using Npgsql;

namespace MiniDDD.UnitOfWork.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private SqlClient<DbConnection> _sqlClient;
        readonly DbContextOptions _options;
        private DbTransaction _dbTransaction;
        public UnitOfWork(DbContextOptions options)
        {
            _options = options;
        }

        public void BeginTransaction()
        {
            var client = GetSqlClient();
            if (client != null && client.Client != null && !client.IsOpenedTransaction)
            {
                if (client.Client.State != System.Data.ConnectionState.Open)
                {
                    client.Client.Open();
                }
                _dbTransaction = client.Client.BeginTransaction();
                client.IsOpenedTransaction = true;
            }
        }

        public void Commit()
        {
            if (_sqlClient != null && _sqlClient.IsOpenedTransaction)
            {
                if (_dbTransaction != null)
                {
                    _dbTransaction.Commit();
                    if (_sqlClient.Client.State != System.Data.ConnectionState.Closed || _sqlClient.Client.State != System.Data.ConnectionState.Broken)
                    {
                        try
                        {
                            _sqlClient.Client.Close();
                        }
                        catch { }
                    }
                }

                _sqlClient.IsOpenedTransaction = false;
            }
        }

        public void Rollback()
        {
            if (_sqlClient != null && _sqlClient.IsOpenedTransaction)
            {
                if (_dbTransaction != null)
                {
                    _dbTransaction.Commit();
                }

                _sqlClient.IsOpenedTransaction = false;
            }

        }

        private SqlClient<DbConnection> GetSqlClient()
        {
            if (_sqlClient == null)
            {
                DbConnection cnn = null;
                switch (_options.DbType)
                {
                    case DbType.MySQL:
                        cnn = new MySqlConnection(_options.ConnectionString);
                        break;
                    case DbType.SQLServer:
                        cnn = new SqlConnection(_options.ConnectionString);
                        break;
                    case DbType.Oracle:
                        cnn = new OracleConnection(_options.ConnectionString);
                        break;
                    case DbType.PostgreSQL:
                        cnn = new NpgsqlConnection(_options.ConnectionString);
                        break;
                    case DbType.SQLite:
                        cnn = new SQLiteConnection(_options.ConnectionString);
                        break;
                    default:
                        throw new Exception("please specify DbType!");
                }
                _sqlClient = new SqlClient<DbConnection>(cnn);
            }
            return _sqlClient;
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(DbConnection)))
            {
                throw new InvalidCastException($"cannot convert {typeof(T)} to DbConnection");
            }
            return GetSqlClient() as SqlClient<T>;
        }


    }
}
