using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiniDDD.UnitOfWork.EF
{


    public class DefaultDbContext : DbContext
    {
        private readonly DbContextOptions _option;
        private string _tableModelAssemblyName;
        private LoggerFactory _loggerFactory;
        readonly Action<LogLevel, string> _logAction;
        readonly LogLevel _logLevel;

        public DefaultDbContext(DbContextOptions option, string tableModelAssemblyName, Action<LogLevel, string> logAction, LogLevel logLevel) : base(new DbContextOptions<DefaultDbContext>())
        {
            _tableModelAssemblyName = tableModelAssemblyName;
            _option = option;
            _logAction = logAction;
            _logLevel = logLevel;
            _loggerFactory = new LoggerFactory();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_logAction != null)
            {
                var provider = new SqlLogProvider(_logAction, _logLevel);
                _loggerFactory.AddProvider(provider);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }
            switch (_option.DbType)
            {
                case DbType.MySQL:
                    optionsBuilder.UseMySql(_option.ConnectionString);
                    break;
                case DbType.SQLServer:
                    optionsBuilder.UseSqlServer(_option.ConnectionString);
                    break;
                case DbType.SQLite:
                    optionsBuilder.UseSqlite(_option.ConnectionString);
                    break;
                case DbType.PostgreSQL:
                    optionsBuilder.UseNpgsql(_option.ConnectionString);
                    break;
                case DbType.Oracle:
                    optionsBuilder.UseOracle(_option.ConnectionString);
                    break;
                default:
                    optionsBuilder.UseMySql(_option.ConnectionString);
                    break;
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddEntityTypes(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void AddEntityTypes(ModelBuilder modelBuilder)
        {
            List<Type> modelTypes = new List<Type>();
            if (!string.IsNullOrEmpty(_tableModelAssemblyName))
            {
                var assembly = Assembly.Load(_tableModelAssemblyName);
                var types = assembly?.GetTypes();
                modelTypes = types?.Where(x => x.IsClass && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDbModel<>))
               && !x.IsGenericType && !x.IsAbstract
               ).ToList();
            }
            else
            {
                modelTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass &&
                          x.GetInterfaces().Any(y => y.IsGenericType &&
                          y.GetGenericTypeDefinition() == typeof(IDbModel<>)) &&
                          !x.IsGenericType && !x.IsAbstract
                          ).ToList();
            }

            if (modelTypes != null && modelTypes.Any())
            {
                modelTypes.ForEach(x =>
                {
                    if (modelBuilder.Model.FindEntityType(x) == null)
                        modelBuilder.Model.AddEntityType(x);
                });
            }
        }

    }
}
