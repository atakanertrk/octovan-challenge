using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OctovanAPI.DataAccess;
using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;

namespace OctovanAPI.Helpers
{
    public class GenerateDetailedInformationOfTaskObject
    {
        IDataAccess _dataAccess;
        public GenerateDetailedInformationOfTaskObject(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public DetailedInformationOfTask Generate(DriverModel driver, UserModel user, TaskModel task, List<string> imageUrls)
        {
            var detailedInformationOfTasks = new DetailedInformationOfTask();
            if (driver != null)
            {
                bool isLiked = _dataAccess.GetUsersLikedTaskIds(user.Id).Contains(task.Id);
                bool isFollowed = _dataAccess.GetUsersFollowedDriverIds(user.Id).Contains(driver.Id);
                var detailedTask = new DetailedInformationOfTask
                {
                    TaskId = task.Id,
                    Driver = new DetailedDriver { Id = task.DriverId, FullName = driver.FullName, PhoneNumber = driver.PhoneNumber, Followed = isFollowed },
                    User = new DetailedUser { Id = task.UserId, FullName = user.FullName, PhoneNumber = user.PhoneNumber },
                    Task = new DetailedTask { Id = task.Id, AssignedDriver = task.DriverId, CreatedAt = task.CreatedAt, Images = imageUrls, Liked = isLiked, Owner = task.UserId }
                };
                return detailedTask;
            }
            else
            {
                bool isLiked = _dataAccess.GetUsersLikedTaskIds(user.Id).Contains(task.Id);
                var detailedTask = new DetailedInformationOfTask
                {
                    TaskId = task.Id,
                    Driver = null,
                    User = new DetailedUser { Id = task.UserId, FullName = user.FullName, PhoneNumber = user.PhoneNumber },
                    Task = new DetailedTask { Id = task.Id, AssignedDriver = task.DriverId, CreatedAt = task.CreatedAt, Images = imageUrls, Liked = isLiked, Owner = task.UserId }
                };
                return detailedTask;
            }
        }
    }
}
