using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MiniDDD.UnitOfWork.EF
{
    public class DefaultDbContext : DbContext
    {
        private readonly DbContextOptions _option;

        public DefaultDbContext(DbContextOptions option) : base(new DbContextOptions<DefaultDbContext>())
        {
            _option = option;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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
            if (!string.IsNullOrEmpty(_option.TableModelAssemblyName))
            {
                var assembly = Assembly.Load(_option.TableModelAssemblyName);
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
