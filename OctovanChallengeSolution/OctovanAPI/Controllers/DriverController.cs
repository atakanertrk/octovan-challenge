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
            _dataAccess.DeleteDriver(driverId);
            return Ok();
        }

        [HttpPost]
        public IActionResult SignIn([FromBody] string PhoneNumber)
        {
            int returnedId = _dataAccess.IsDriverExistByPhoneNumber(PhoneNumber);
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
        public IActionResult AssignToTask([FromBody] TaskIdAndDriverId ids)
        {
            _dataAccess.UpdateTaskAddDriver(ids.TaskId, ids.DriverId);
            return Ok();
        }
    }
}
