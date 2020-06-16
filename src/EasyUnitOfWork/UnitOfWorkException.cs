using System;
using System.Collections.Generic;
using System.Text;

namespace EasyUnitOfWork
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException() { }
        public UnitOfWorkException(string msg) : base(msg) { }
        public UnitOfWorkException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
