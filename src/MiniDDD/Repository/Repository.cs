﻿using System;
using System.Linq;

namespace MiniDDD
{
    public abstract class Repository<T, TKey> :
        InlineEventHandler<TKey>,
        IRepository<T, TKey>
        where T : AggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        public abstract T Get(TKey key);

        public virtual void Save(T aggreateRoot)
        {
            if (aggreateRoot.UncommittedEvents.Any())
            {
                foreach (var e in aggreateRoot.UncommittedEvents)
                {
                    ApplyEvent(e);
                }
            }
        }
    }
}
