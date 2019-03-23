using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDD.Simple.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }
        // GET: api/User
        [HttpGet]
        public IEnumerable<string> Get()
        {
            this._userService.CreateUser(new DDD.Simple.IServices.DTO.UserCreateReq
            {
                Name = "grissom333",
                Email = "gg@qq.com",
                ReqId = new Guid()
            });
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
