using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices.Commands;
using Jimu;

namespace DDD.CQRS.IServices
{
    [JimuServiceRoute("api/{Service}")]
    public interface IUserService : IJimuService
    {
        [JimuService(CreatedBy = "grissom", Comment = "getuser")]
        string GetUser();
        [JimuService(CreatedBy = "grissom", Comment = "create user")]
        Task<Guid> CreateUser(CreateUser createUser);
    }
}
