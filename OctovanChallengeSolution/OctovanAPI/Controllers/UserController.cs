using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctovanAPI.DataAccess;
using OctovanAPI.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SqlServerDataAccess _dataAccess;
        public UserController(IConfiguration config)
        {
            _dataAccess = new SqlServerDataAccess(config);
        }

        [HttpPost]
        public IActionResult InsertUser([FromBody] UserDTO user)
        {
            int userId = _dataAccess.InsertUser(user);
            return Ok(new { userId = userId });
        }

        [HttpDelete]
        public IActionResult DeleteUser([FromQuery] int userId)
        {
            _dataAccess.DeleteAllTasksOfUser(userId);
            _dataAccess.DeleteUser(userId);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = _dataAccess.GetAllUsers();
            return Ok(result);
        }
       
        [HttpPost]
        public IActionResult SignIn([FromBody] SignIn signin)
        {
            int returnedId = _dataAccess.IsUserExistByPhoneNumber(signin.PhoneNumber);
            if (returnedId > 0)
            {
                return Ok(new { userId = returnedId });
            }
            else if (returnedId == 0)
            {
                return Unauthorized();
            }
            return BadRequest();
        }
      
       
    }
}
