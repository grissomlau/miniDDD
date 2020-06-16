using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EasyUnitOfWork.EF
{


    public class DefaultDbContext : DbContext
    {
        private readonly DbContextOptions _options;
        private string _tableModelAssemblyName;
        private LoggerFactory _loggerFactory;
        readonly Action<LogLevel, string> _logAction;
        readonly LogLevel _logLevel;
        List<Type> _modelTypes;

        List<Type> ModelTypes
        {
            get
            {
                if (_modelTypes == null)
                {
                    _modelTypes = new List<Type>();
                    if (!string.IsNullOrEmpty(_tableModelAssemblyName))
                    {
                        var assembly = Assembly.Load(_tableModelAssemblyName);
                        var types = assembly?.GetTypes();
                        _modelTypes = types?.Where(x => x.IsClass && x.GetInterface(typeof(IEFEntity).FullName) != null
                       && !x.IsGenericType && !x.IsAbstract
                       ).ToList();
                    }
                    else
                    {
                        _modelTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                            .Where(x => x.IsClass &&
                                  x.GetInterface(typeof(IEFEntity).FullName) != null &&
                                  !x.IsGenericType && !x.IsAbstract
                                  ).ToList();
                    }
                }
                return _modelTypes;
            }
        }

        public DefaultDbContext(DbContextOptions option, string tableModelAssemblyName, Action<LogLevel, string> logAction, LogLevel logLevel) : base(new DbContextOptions<DefaultDbContext>())
        {
            _tableModelAssemblyName = tableModelAssemblyName;
            _options = option;
            _logAction = logAction;
            _logLevel = logLevel;
            _loggerFactory = new LoggerFactory();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(_options.ProviderName))
            {
                throw new UnitOfWorkException("DbContextOptions.ProviderName cannot be null or empty, please specify the EFProvider");
            }

            if (_logAction != null)
            {
                var provider = new SqlLogProvider(_logAction, _logLevel);
                _loggerFactory.AddProvider(provider);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }

            var efProvider = EFProviderContainer.Instance.GetFactory(_options.ProviderName);

            if (efProvider == null)
            {
                throw new UnitOfWorkException($"the DbContextOptions.ProviderName:  {_options.ProviderName}, not Register in EFProviderFactories");
            }

            efProvider.Use(optionsBuilder, _options.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddEntityTypes(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void AddEntityTypes(ModelBuilder modelBuilder)
        {
            ModelTypes.ForEach(x =>
            {
                if (modelBuilder.Model.FindEntityType(x) == null)
                    //modelBuilder.Model.AddEntityType(x);
                    //modelBuilder.Entity(x).HasNoKey();
                    modelBuilder.Entity(x);
            });
        }

    }
}
