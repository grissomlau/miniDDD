using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDDD.UnitOfWork.EF
{
    public interface IEFProvider
    {
        void Use(DbContextOptionsBuilder optionsBuilder, string connectionString);
    }
}
