using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;

namespace OctovanAPI.Helpers
{
    public class GenerateDetailedInformationOfTaskObject
    {
        public DetailedInformationOfTask Generate(DriverModel driver, UserModel user, TaskModel task, List<string> imageUrls, bool isFollowed, bool isLiked)
        {
            var detailedInformationOfTasks = new DetailedInformationOfTask();
            if (driver != null)
            {
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
