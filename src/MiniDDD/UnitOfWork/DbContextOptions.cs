using System;

namespace MiniDDD.UnitOfWork
{
    public class DbContextOptions
    {
        public string ConnectionString { get; set; }
        public DbType DbType { get; set; }
    }


}
