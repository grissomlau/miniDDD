using DDD.Simple.Domain;
using MiniDDD;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using MiniDDD.UnitOfWork;
using DDD.Simple.Model;
using System.Linq;
using DDD.Simple.Domain.Events;

namespace Simple.Repository.Dapper
{
    public class UserRepository : Repository<DDD.Simple.Domain.User, Guid>
    {
        DDD.Simple.Model.User _user;
        readonly DbConnection _dbConnection;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.GetSqlClient<DbConnection>();
        }
        public override DDD.Simple.Domain.User Get(Guid key)
        {
            var userModel = _dbConnection.QuerySingleOrDefault<DDD.Simple.Model.User>("Select * From user Where Id = @id", new { id = key });
            if (userModel == null)
                return null;
            var userFriendModels = _dbConnection.Query<DDD.Simple.Model.UserFriend>("Select * From userfriend where UserId = @userid", new { userid = key });
            var user = DDD.Simple.Domain.User.Load(userModel.Id, userModel.Name, userModel.Email, userFriendModels.Select(x => x.FriendUserId).ToList());
            return user;
        }

        public override void Save(DDD.Simple.Domain.User aggreateRoot)
        {
            base.Save(aggreateRoot);
            _user = null;
        }

        private DDD.Simple.Model.User GetUserModel(IDomainEvent<Guid> e)
        {
            if (_user == null)
            {
                _user = _dbConnection.QuerySingleOrDefault<DDD.Simple.Model.User>("Select * From user Where Id = @id", new { id = e.AggregateRootKey });

            }

            return _user ?? (_user = new DDD.Simple.Model.User());
        }

        [InlineEventHandler]
        private void HandleUserRegistered(UserRegistered e)
        {
            var userModel = GetUserModel(e);
            userModel.Id = e.Id;
            userModel.Name = e.Name;
            userModel.Email = e.Email;
            _dbConnection.Execute("insert into user(Id, Name, Email) values(@id,@name, @email)", new { id = e.AggregateRootKey, name = e.Name, email = e.Email });
            //_dbConnection.Set<DDD.Simple.Model.User>().Add(userModel);
            //this._dbContext.Entry<Model.User>(userModel).Reload();
            //_dbContext.Add(userModel);
        }

        [InlineEventHandler]
        private void HandleUserNameChanged(UserNameChanged e)
        {
            var userModel = GetUserModel(e);
            userModel.Name = e.Name;
            _dbConnection.Execute("update user set Name = @name where id = @id", new { name = e.Name, id = e.AggregateRootKey });
            //var entry = _dbConnection.Entry(userModel);

            //if (entry.State != EntityState.Added)
            //{
            //    entry.Property(x => x.Name).IsModified = true;
            //}
            //_dbContext.Set<Model.User>().Update(userModel);
        }
    }
}
