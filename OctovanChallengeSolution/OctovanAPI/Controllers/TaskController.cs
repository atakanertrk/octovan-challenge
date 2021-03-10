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

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var result = _dataAccess.GetAllTasks();
            return Ok(result);
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
        
        [HttpGet]
        public IActionResult TasksOfDriver([FromQuery] int driverId)
        {
            var taasks = _dataAccess.GetTasksOfDriver(driverId);
            return Ok(taasks);
        }

        // task/newsfeed ? userId -> returns list of tasks from drivers that user is following in chronological order
        [HttpGet]
        public IActionResult NewsFeed([FromQuery] int userId)
        {
            // user'ın takip ettiği tüm driverların id'si ni al
            List<int> driverIds = _dataAccess.GetUsersFollowedDriverIds(userId);
            // herbir driverid'si için driverid'si içeren taskları al ve bir listede topla
            List<TaskModel> allTasks = new List<TaskModel>();
            foreach (int driverId in driverIds)
            {
                List<TaskModel> tasks = _dataAccess.GetTasksOfDriver(driverId);
                foreach (TaskModel task in tasks)
                {
                    allTasks.Add(task);
                }
            }
            // listeyi createdAt e göre azalan sırada sırala
            var allTasksOrdered = allTasks.OrderByDescending(x=>x.CreatedAt);
            return Ok(allTasksOrdered);
        }

        // task / GetTotalLikesOfTask ? taskid : 3 returns int
        [HttpGet]
        public IActionResult TotalLikesOfTask([FromQuery] int taskId)
        {
           int totalLikes = _dataAccess.GetTotalLikesOfTask(taskId);
           return Ok(new { totalLikes = totalLikes, taskId = taskId});
        }

        // task/deletetask ? taskid : 2
        [HttpDelete]
        public IActionResult DeleteTask([FromQuery] int taskId)
        {
            _dataAccess.DeleteTaskByTaskId(taskId);
            _blobService.DeleteContainer(taskId.ToString());
            return Ok();
        }

       // task/getallinformationByGivenTaskIds { listoftaskid }  returns: foreach of id, create nested objects as declaterd. and return List<DetailedInformationOfTask> object
       // assume given taskids are uniqe ( so, no worries about this case )
       // must return a list of tasks in the same order as taskIds
       // place null if task is not exist
        [HttpPost]
        public async Task<IActionResult> AllInformationByGivenTaskIds([FromBody] ListOfTaskIdAndUserId ids)
        {
            var detailedInformationOfTasks = new List<DetailedInformationOfTask>();
            foreach (int taskId in ids.TaskIds)
            {
                bool isFollowed = _dataAccess.GetUsersFollowedDriverIds(ids.UserId).Contains(taskId);
                bool isLiked = _dataAccess.GetUsersLikedTaskIds(ids.UserId).Contains(taskId);
                TaskModel task = _dataAccess.GetTask(taskId);
                DriverModel driver = _dataAccess.GetDriver(task.DriverId);
                UserModel user = _dataAccess.GetUser(task.UserId);
                List<string> imageUrls = (List<string>)await _blobService.ListBlobsUrlAsync(taskId.ToString());
                var detailedTask = new DetailedInformationOfTask
                {
                    TaskId = taskId,
                    Driver = new DetailedDriver { Id= task.DriverId, FullName=driver.FullName , PhoneNumber= driver.PhoneNumber,Followed= isFollowed },
                    User = new DetailedUser { Id= task.UserId,  FullName= user.FullName , PhoneNumber=user.PhoneNumber },
                    Task = new DetailedTask { Id= task.Id , AssignedDriver = task.DriverId, CreatedAt = task.CreatedAt, Images = imageUrls, Liked = isLiked, Owner = ids.UserId }
                };
                detailedInformationOfTasks.Add(detailedTask);
            }
            return Ok(detailedInformationOfTasks);
        }
    }
}
