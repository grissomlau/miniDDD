using System;

namespace MiniDDD
{
    public interface IDomainEvent<TKey>
    {
        Guid Id { get; }
        TKey AggregateRootKey { get; set; }

        DateTime Timestamp { get; }
    }
}
