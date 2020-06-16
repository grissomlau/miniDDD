using System;
using System.ComponentModel.DataAnnotations.Schema;
using EasyUnitOfWork;
using SqlSugar;

namespace DDD.Simple.Model
{
    [Table("UserFriend")]
    [SugarTable("UserFriend")]
    public class UserFriend : IEFEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendUserId { get; set; }
    }
}
