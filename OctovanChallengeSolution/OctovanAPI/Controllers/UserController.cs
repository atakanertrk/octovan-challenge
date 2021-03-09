using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctovanAPI.DataAccess;
using OctovanAPI.Helpers;
using OctovanAPI.Models;
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
            _dataAccess.DeleteUser(userId);
            return Ok();
        }

        [HttpPost]
        public IActionResult SignIn([FromBody] string PhoneNumber)
        {
            int returnedId = _dataAccess.IsUserExistByPhoneNumber(PhoneNumber);
            if (returnedId > 0)
            {
                return Ok();
            }
            else if (returnedId == 0)
            {
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult CreateNewTask([FromBody] CreateTaskDTO task)
        {
            var taskModel = new TaskModel();
            taskModel.CreatedAt = UnixTimestampHelper.GetCurrentTimestamp();
            taskModel.Description = task.Description;
            taskModel.UserId = task.UserId;
            _dataAccess.InsertNewTask(taskModel);
            return Ok();
        }

    }
}
