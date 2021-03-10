using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;
using System.Collections.Generic;

namespace OctovanAPI.DataAccess
{
    public interface IDataAccess
    {
        void DeleteAllTasksOfDriver(int driverId);
        void DeleteTaskByTaskId(int taskId);
        List<DriverModel> GetAllDrivers();
        List<TaskModel> GetAllTasks();
        List<UserModel> GetAllUsers();
        DriverModel GetDriver(int driverId);
        TaskModel GetTask(int taskId);
        List<TaskModel> GetTasksOfDriver(int driverId);
        int GetTotalLikesOfTask(int taskId);
        UserModel GetUser(int userId);
        List<int> GetUsersFollowedDriverIds(int userId);
        List<int> GetUsersLikedTaskIds(int userId);
        int InsertDriver(DriverDTO driver);
        void InsertFollow(UserIdAndDriverId ids);
        void InsertLikeToLikesOfTasks(TaskIdAndUserId ids);
        int InsertNewTask(TaskModel task);
        int InsertUser(UserDTO user);
        bool IsDriverExistByDriverId(int driverId);
        int IsDriverExistByPhoneNumber(string phoneNumber);
        bool IsTaskExistById(int taskId);
        int IsUserExistByPhoneNumber(string phoneNumber);
        void UpdateTaskAddDriver(int taskId, int driverId);
    }
}