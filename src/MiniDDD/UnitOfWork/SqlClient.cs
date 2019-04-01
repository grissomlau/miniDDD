using System;

namespace MiniDDD.UnitOfWork
{
    public class SqlClient<T> where T : class
    {
        public T Client { get; set; }
        public bool IsOpenedTransaction { get; set; }
        public Guid Id { get; set; }

        public SqlClient(T client)
        {
            Client = client;
            Id = Guid.NewGuid();
        }
        event Action<string> OnExportingSql;
    }
}
