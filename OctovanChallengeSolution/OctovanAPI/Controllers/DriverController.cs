using Microsoft.AspNetCore.Http;
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
    public class DriverController : ControllerBase
    {
        private SqlServerDataAccess _dataAccess;
        public DriverController(IConfiguration config)
        {
            _dataAccess = new SqlServerDataAccess(config);
        }

        [HttpPost]
        public IActionResult InsertDriver([FromBody] DriverDTO driver)
        {
            int driverId = _dataAccess.InsertDriver(driver);
            return Ok(new { driverId = driverId});
        }

        [HttpDelete]
        public IActionResult DeleteDriver([FromQuery] int driverId)
        {
            _dataAccess.DeleteAllTasksOfDriver(driverId);
            _dataAccess.DeleteDriver(driverId);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetAllDrivers()
        {
            var result = _dataAccess.GetAllDrivers();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult SignIn([FromBody] SignIn signin)
        {
            int returnedId = _dataAccess.IsDriverExistByPhoneNumber(signin.PhoneNumber);
            if (returnedId > 0)
            {
                return Ok(new { driverId = returnedId });
            }
            else if (returnedId == 0)
            {
                return Unauthorized();
            }
            return BadRequest();
        }
    }
}
