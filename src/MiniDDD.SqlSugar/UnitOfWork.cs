using System;
using Sugar = SqlSugar;
using System.Linq;
using SqlSugar;

namespace MiniDDD.UnitOfWork.SqlSugar
{
    public class UnitOfWork : IUnitOfWork
    {
        //private readonly ConcurrentDictionary<string, SqlClient<SqlSugarClient>> _sqlClients = new ConcurrentDictionary<string, SqlClient<SqlSugarClient>>();
        //private readonly ThreadLocal<string> _threadLocal;
        /// <summary>
        /// diffrent vaue for every thread
        /// </summary>
        //private ThreadLocal<SqlClient<SqlSugarClient>> _tlSqlClient;
        private readonly DbContextOptions _options;
        private readonly Action<string> _logAction;
        private bool _disposed = false;

        private SqlSugarClient _client;
        public T GetSqlClient<T>() where T : class
        {
            if (_client == null)
            {
                _client = new Sugar.SqlSugarClient(new Sugar.ConnectionConfig
                {
                    IsAutoCloseConnection = true,
                    ConnectionString = _options.ConnectionString,
                    DbType = (Sugar.DbType)Enum.Parse(typeof(Sugar.DbType), _options.DbType.ToString(), true),
                    InitKeyType = Sugar.InitKeyType.SystemTable

                });
                if (_logAction != null)
                {
                    _client.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        var paraStr = "";
                        if (pars != null && pars.Any())
                        {
                            paraStr = Environment.NewLine + ", parameters: " + string.Join(Environment.NewLine + ",", pars.Select(x => $"DbType: {x.DbType} - Name: { x.ParameterName} - Value: { x.Value}"));
                        }
                        _logAction($"{sql}{paraStr}");
                        //Console.WriteLine($"SQL: {sql}");
                    };
                }
            }

            if (!typeof(T).IsAssignableFrom(typeof(Sugar.SqlSugarClient)))
            {
                throw new InvalidCastException($"cannot convert {typeof(T)} to SqlSugarClient");
            }
            return _client as T;
        }
        public UnitOfWork(DbContextOptions options, Action<string> logAction)
        {
            _options = options;
            _logAction = logAction;
        }
        public void BeginTransaction()
        {
            _client.Ado.BeginTran();
        }

        public void Commit()
        {
            _client.Ado.CommitTran();
        }


        public void Rollback()
        {
            _client.Ado.RollbackTran();
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

                if (_client != null)
                {
                    try
                    {
                        _client.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _client = null;
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
