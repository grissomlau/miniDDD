using System;
using System.Collections.Generic;

namespace MiniDDD
{
    public interface IAggregateRoot<TKey> :
        IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IEnumerable<IDomainEvent> UncommittedEvents { get; }

        void Replay(IEnumerable<IDomainEvent> events);
    }
}
