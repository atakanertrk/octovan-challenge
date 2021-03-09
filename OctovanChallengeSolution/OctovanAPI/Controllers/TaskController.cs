using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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
    public class TaskController : ControllerBase
    {
        private SqlServerDataAccess _dataAccess;
        private readonly IHostingEnvironment _environment;
        private readonly IBlobService _blobService;
        private readonly BlobServiceClient _blobServiceClient;
        public TaskController(IConfiguration config, IHostingEnvironment environment, IBlobService blobService, BlobServiceClient blobServiceClient)
        {
            _blobService = blobService;
            _environment = environment;
            _dataAccess = new SqlServerDataAccess(config);
            _blobServiceClient = blobServiceClient;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImageFromTask([FromBody] DeleteImageDTO delete)
        {
            await _blobService.DeleteBlobAsync(delete.FileName, delete.TaskId.ToString());
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetImagesOfTask([FromQuery] int taskId)
        {
            var listOfTaskImages = await _blobService.ListBlobsUrlAsync(taskId.ToString());
            return Ok(listOfTaskImages);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageForTask([FromQuery] int taskId)
        {
            string fileName = "";
            var files = HttpContext.Request.Form.Files;
            if (HttpContext.Request.Form.Files != null)
            {
                if (files[0].Length > 0)
                {
                    fileName = files[0].FileName;
                    var containerClient = _blobServiceClient.GetBlobContainerClient("taskid-" + taskId.ToString());
                    var blobClient = containerClient.GetBlobClient(fileName);
                    using (var ms = new MemoryStream())
                    {
                        files[0].CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        using (var ms2 = new MemoryStream(fileBytes))
                        {
                            await blobClient.UploadAsync(ms2, new BlobHttpHeaders { ContentType = "application/octet-stream" });
                        }
                    }
                }
                return Ok();
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
            int createdTaskId = _dataAccess.InsertNewTask(taskModel);
            if (createdTaskId != 0)
            {
                await _blobService.CreateContainerForTask(createdTaskId.ToString());
                return Ok(new { createdTaskId = createdTaskId});
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult AssignDriverToTask([FromBody] TaskIdAndDriverId ids)
        {
            _dataAccess.UpdateTaskAddDriver(ids.TaskId, ids.DriverId);
            return Ok();
        }



    }
}
