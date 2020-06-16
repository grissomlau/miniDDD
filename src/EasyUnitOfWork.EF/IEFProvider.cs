using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyUnitOfWork.EF
{
    public interface IEFProvider
    {
        void Use(DbContextOptionsBuilder optionsBuilder, string connectionString);
    }
}
