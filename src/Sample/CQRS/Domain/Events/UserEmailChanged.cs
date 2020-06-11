
using MiniDDD;
using System;

namespace DDD.CQRS.Domain.Events
{
    public class UserEmailChanged : DomainEvent<Guid>
    {
        public string Email { get; set; }
        public UserEmailChanged(Guid aggregateRootKey, string email) : base(aggregateRootKey)
        {
            Email = email;
        }
    }
}
