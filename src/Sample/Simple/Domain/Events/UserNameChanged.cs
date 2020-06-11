
using MiniDDD;
using System;

namespace DDD.Simple.Domain.Events
{
    public class UserNameChanged : DomainEvent<Guid>
    {
        public string Name { get; set; }
        public UserNameChanged(Guid aggregateRootKey, string name) : base(aggregateRootKey)
        {
            Name = name;
        }
    }
}
