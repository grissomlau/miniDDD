using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDD.Simple.IServices.DTO;
using Jimu;

namespace DDD.Simple.IServices
{
    [JimuServiceRoute("api/simple/{Service}")]
    public interface IUserService : IJimuService
    {
        [JimuService(CreatedBy = "grissom", Comment = "getuser", EnableAuthorization = true)]
        UserDto GetUser(Guid id);
        [JimuService(CreatedBy = "grissom", Comment = "create user")]
        Task<Guid> CreateUser(UserCreateReq userCreateReq);
        [JimuService(CreatedBy = "grissom", Comment = "get user by name")]
        Task<UserDto> GetByName(string userName);

        [JimuService(CreatedBy = "grissom", Comment = "change user name")]
        Task<Guid> UpdateUser(UserNameChangeReq userNameChangeReq);


        [JimuService(CreatedBy = "grissom", Comment = "get file")]
        //Task<Byte[]> GetFile();
        Task<JimuFile> GetFile();

        [JimuService(CreatedBy = "grissom", Comment = "get file")]
        Task UploadFiles(List<JimuFile> files);
    }
}
