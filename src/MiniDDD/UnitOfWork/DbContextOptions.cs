using System;

namespace MiniDDD.UnitOfWork
{
    public class DbContextOptions
    {
        public string ConnectionString { get; set; }
        /// <summary>
        /// DbProviderName(for Sql) or EFProviderName(for EF) or DbType(for SqlSugar)
        /// </summary>
        public string ProviderName { get; set; }
    }
}
