using System;

namespace MiniDDD
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        object AggregateRootKey { get; set; }

        DateTime Timestamp { get; }
    }
}
