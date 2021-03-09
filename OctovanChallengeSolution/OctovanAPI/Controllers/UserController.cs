using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctovanAPI.DataAccess;
using OctovanAPI.Helpers;
using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;
using OctovanAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SqlServerDataAccess _dataAccess;

        private readonly IHostingEnvironment _environment;
        private readonly IBlobService _blobService;
        public UserController(IConfiguration config, IHostingEnvironment environment, IBlobService blobService)
        {
            _blobService = blobService;
            _environment = environment;
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
        public async Task<IActionResult> CreateNewTask([FromBody] CreateTaskDTO task)
        {
            var taskModel = new TaskModel();
            taskModel.CreatedAt = UnixTimestampHelper.GetCurrentTimestamp();
            taskModel.Description = task.Description;
            taskModel.UserId = task.UserId;
            _dataAccess.InsertNewTask(taskModel);
            await _blobService.CreateContainerForTask(task.UserId.ToString());
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> UploadImageForTask([FromQuery] string taskId)
        {
            string filePath = "";
            string fileName = "";
            var files = HttpContext.Request.Form.Files;
            if (HttpContext.Request.Form.Files != null)
            {
                if (files[0].Length > 0)
                {
                    fileName = files[0].FileName;
                    filePath = Path.Combine(_environment.WebRootPath, @"temp") + $@"\{fileName}";
                    using (FileStream fs = System.IO.File.Create(filePath))
                    {
                        files[0].CopyTo(fs);
                        fs.Flush();
                    }
                }
                try
                {
                    await _blobService.UploadFileBlobAsync(filePath, fileName, taskId);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    System.IO.File.Delete(filePath);
                }
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImageFromTask([FromBody] DeleteImageDTO delete)
        {
            await _blobService.DeleteBlobAsync(delete.FileName,delete.TaskId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetImagesOfTask([FromQuery] string taskId)
        {
            var listOfTaskImages = await _blobService.ListBlobsAsync(taskId);
            return Ok(listOfTaskImages);
        }
    }
}
