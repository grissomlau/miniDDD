using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDD.Simple.IServices.DTO;

namespace DDD.Simple.IServices
{
    public interface IAutoInject { }
    public interface IUserService : IAutoInject
    {
        UserDto GetUser(Guid id);
        Task<Guid> CreateUser(UserCreateReq userCreateReq);
        Task<UserDto> GetByName(string userName);

        Task<Guid> UpdateUser(UserNameChangeReq userNameChangeReq);


        //Task<Byte[]> GetFile();
        //Task<JimuFile> GetFile();

        //Task UploadFiles(List<JimuFile> files);
    }
}
