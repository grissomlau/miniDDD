using EasyUnitOfWork.EF;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway
{
    public class MysqlEFProvider : IEFProvider
    {
        public void Use(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseMySql(connectionString);
        }
    }
}
