using System;
using Sugar = SqlSugar;
using System.Linq;

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
        private SqlClient<Sugar.SqlSugarClient> _sqlClient;
        private readonly DbContextOptions _options;
        private readonly Action<string> _logAction;

        public UnitOfWork(DbContextOptions options, Action<string> logAction)
        {
            _options = options;
            _logAction = logAction;
        }
        public void BeginTransaction()
        {
            var sqlClient = GetSqlClient();
            if (sqlClient?.Client != null)
            {
                if (!sqlClient.IsOpenedTransaction)
                    sqlClient.Client.Ado.BeginTran();
                sqlClient.IsOpenedTransaction = true;
            }
        }

        public void Commit()
        {
            if (_sqlClient?.Client != null && _sqlClient.IsOpenedTransaction)
            {
                _sqlClient.IsOpenedTransaction = false;
                _sqlClient.Client.Ado.CommitTran();
            }
        }

        private SqlClient<Sugar.SqlSugarClient> GetSqlClient()
        {
            if (_sqlClient == null)
            {
                var client = new Sugar.SqlSugarClient(new Sugar.ConnectionConfig
                {
                    IsAutoCloseConnection = true,
                    ConnectionString = _options.ConnectionString,
                    DbType = (Sugar.DbType)Enum.Parse(typeof(Sugar.DbType), _options.DbType.ToString(), true),
                    InitKeyType = Sugar.InitKeyType.SystemTable

                });
                if (_logAction != null)
                {
                    client.Aop.OnLogExecuting = (sql, pars) =>
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
                _sqlClient = new SqlClient<Sugar.SqlSugarClient>(client);
            }
            return _sqlClient;
        }

        public void Rollback()
        {
            if (_sqlClient?.Client != null && _sqlClient.IsOpenedTransaction)
            {
                _sqlClient.Client.Ado.RollbackTran();
            }
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(Sugar.SqlSugarClient)))
            {
                throw new InvalidCastException($"cannot convert {typeof(T)} to SqlSugarClient");
            }
            return GetSqlClient() as SqlClient<T>;
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
    }
}
