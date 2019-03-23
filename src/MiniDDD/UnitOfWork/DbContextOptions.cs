using System;

namespace MiniDDD.UnitOfWork
{
    public class DbContextOptions
    {
        public string ConnectionString { get; set; }
        public string TableModelAssemblyName { get; set; }
        public DbType DbType { get; set; }
    }

    [Flags]
    public enum DbType
    {
        MySQL,
        SQLServer,
        Oracle,
        PostgreSQL,
        SQLite,
        //MongoDb,
        //Redis,
        //Memcached
    }
}
