using System;
using System.Linq;
using DDD.Simple.Domain.Events;
using DDD.Simple.Model;
using EasyUnitOfWork;
using Microsoft.EntityFrameworkCore;
using MiniDDD;
using User = DDD.Simple.Domain.User;

namespace DDD.Simple.Repository.EF
{
    public class UserRepository : Repository<User, Guid>
    {
        readonly DbContext _dbContext;
        Model.User _user;
        public UserRepository(IUnitOfWork unitOfWork)
        {
            _dbContext = unitOfWork.GetUowWorker<DbContext>();
        }
        public override User Get(Guid key)
        {
            var userModel = _dbContext.Set<Model.User>().AsQueryable().Where(x => x.Id == key).SingleOrDefault(); ;
            if (userModel == null)
                return null;
            var userFriendModels = _dbContext.Set<UserFriend>().AsQueryable().Where(x => x.UserId == key).ToList();
            var user = User.Load(userModel.Id, userModel.Name, userModel.Email, userFriendModels.Select(x => x.FriendUserId).ToList());
            return user;
        }

        public override void Save(User aggreateRoot)
        {
            base.Save(aggreateRoot);
            _user = null;
        }

        private Model.User GetUserModel(IDomainEvent<Guid> e)
        {
            if (_user == null)
            {
                _user = _dbContext.Set<Model.User>().AsQueryable().FirstOrDefault(x => x.Id == (Guid)e.AggregateRootKey);
            }

            return _user ?? (_user = new Model.User());
        }

        [InlineEventHandler]
        private void HandleUserRegistered(UserRegistered e)
        {
            var userModel = GetUserModel(e);
            userModel.Id = e.Id;
            userModel.Name = e.Name;
            userModel.Email = e.Email;
            _dbContext.Set<Model.User>().Add(userModel);
            //_dbContext.Set<Model.OrderItem>().Add(new OrderItem { Id = Guid.NewGuid() });
            //this._dbContext.Entry<Model.User>(userModel).Reload();
            //_dbContext.Add(userModel);
        }

        [InlineEventHandler]
        private void HandleUserNameChanged(UserNameChanged e)
        {
            var userModel = GetUserModel(e);
            userModel.Name = e.Name;
            var entry = _dbContext.Entry(userModel);

            //if (entry.State != EntityState.Added)
            //{
            //    entry.Property(x => x.Name).IsModified = true;
            //}
            //_dbContext.Set<Model.User>().Update(userModel);
        }
    }
}
