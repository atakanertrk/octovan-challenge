using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;
using System.Collections.Generic;

namespace OctovanAPI.DataAccess
{
    public interface IDataAccess
    {
        void DeleteAllTasksOfDriver(int driverId);
        void DeleteAllTasksOfUser(int userId);
        void DeleteDriver(int id);
        void DeleteTaskByTaskId(int taskId);
        void DeleteUser(int id);
        List<DriverModel> GetAllDrivers();
        List<UserModel> GetAllUsers();
        int InsertDriver(DriverDTO driver);
        int InsertNewTask(TaskModel task);
        int InsertUser(UserDTO user);
        int IsDriverExistByPhoneNumber(string phoneNumber);
        int IsUserExistByPhoneNumber(string phoneNumber);
        void UpdateTaskAddDriver(int taskId, int driverId);
    }
}