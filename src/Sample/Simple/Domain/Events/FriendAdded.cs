using MiniDDD;
using System;

namespace DDD.Simple.Domain.Events
{
    public class FriendAdded : DomainEvent<int>
    {
        public Guid FriendUserId { get; set; }
        public FriendAdded(int aggregateRootKey, Guid friendUserId) : base(aggregateRootKey)
        {
            FriendUserId = friendUserId;
        }
    }
}
