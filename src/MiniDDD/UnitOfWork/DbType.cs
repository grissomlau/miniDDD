using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDDD.UnitOfWork
{
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
