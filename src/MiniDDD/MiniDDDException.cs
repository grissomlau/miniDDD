using System;
using System.Collections.Generic;
using System.Text;

namespace MiniDDD
{
    public class MiniDDDException : Exception
    {
        public MiniDDDException() { }
        public MiniDDDException(string msg) : base(msg) { }
        public MiniDDDException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
