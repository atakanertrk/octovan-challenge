using Dapper;
using Microsoft.Extensions.Configuration;
using OctovanAPI.Models;
using OctovanAPI.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.DataAccess
{
    public class SqlServerDataAccess : IDataAccess
    {
        private string _conStr;
        public SqlServerDataAccess(IConfiguration config)
        {
            _conStr = ConfigurationExtensions.GetConnectionString(config, "SqlServerConnectionString");
        }

        public int InsertUser(UserDTO user)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@PhoneNumber", user.PhoneNumber);
                p.Add("@FullName", user.FullName);
                p.Add("@Email", user.Email);

                string sql = "INSERT INTO Users (PhoneNumber,FullName,Email) VALUES (@PhoneNumber,@FullName,@Email); SELECT SCOPE_IDENTITY();";

                return cnn.Query<int>(sql, p).ToList().First();
            }
        }

        public int InsertDriver(DriverDTO driver)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@PhoneNumber", driver.PhoneNumber);
                p.Add("@FullName", driver.FullName);

                string sql = "INSERT INTO Drivers (PhoneNumber,FullName) VALUES (@PhoneNumber,@FullName); SELECT SCOPE_IDENTITY();";

                return cnn.Query<int>(sql, p).ToList().First();
            }
        }

        public void DeleteUser(int id)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);

                string sql = "DELETE FROM Users WHERE Id=@Id;";

                cnn.Execute(sql, p);
            }
        }

        public void DeleteDriver(int id)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);

                string sql = "DELETE FROM Drivers WHERE Id=@Id;";

                cnn.Execute(sql, p);
            }
        }

        public List<UserModel> GetAllUsers()
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                string sql = "SELECT * FROM Users;";

                var result = cnn.Query<UserModel>(sql).ToList();
                return result;
            }
        }

        public List<DriverModel> GetAllDrivers()
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                string sql = "SELECT * FROM Drivers;";

                var result = cnn.Query<DriverModel>(sql).ToList();
                return result;
            }
        }

        /// <summary>
        /// returns 0 if user not exist, otherwise returns user id
        /// </summary>
        public int IsUserExistByPhoneNumber(string phoneNumber)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@PhoneNumber", phoneNumber);

                string sql = "SELECT Id FROM Users WHERE PhoneNumber=@PhoneNumber;";

                int result = cnn.Query<int>(sql, p).ToList().FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// returns 0 if driver not exist, otherwise returns user id
        /// </summary>
        public int IsDriverExistByPhoneNumber(string phoneNumber)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@PhoneNumber", phoneNumber);

                string sql = "SELECT Id FROM Drivers WHERE PhoneNumber=@PhoneNumber;";

                int result = cnn.Query<int>(sql, p).ToList().FirstOrDefault();
                return result;
            }
        }

        public int InsertNewTask(TaskModel task)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Description", task.Description);
                p.Add("@CreatedAt", task.CreatedAt);
                p.Add("@UserId", task.UserId);
                string sql = "INSERT INTO Tasks ([Description],CreatedAt,UserId) VALUES (@Description,@CreatedAt,@UserId); SELECT SCOPE_IDENTITY();";

                int result = cnn.Query<int>(sql, p).ToList().FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// deletes all tasks which includes given UserId
        /// </summary>
        public void DeleteAllTasksOfUser(int userId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@UserId", userId);

                string sql = "DELETE FROM Tasks WHERE UserId=@UserId;";

                cnn.Execute(sql, p);
            }
        }

        public void DeleteTaskByTaskId(int taskId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", taskId);

                string sql = "DELETE FROM Tasks WHERE Id=@Id;";

                cnn.Execute(sql, p);
            }
        }

        /// <summary>
        /// deletes all tasks which includes given DiverId
        /// </summary>
        public void DeleteAllTasksOfDriver(int driverId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@DriverId", driverId);

                string sql = "DELETE FROM Tasks WHERE DriverId=@DriverId;";

                cnn.Execute(sql, p);
            }
        }

        /// <summary>
        /// Sets DriverId to existing Task by TaskId
        /// </summary>
        public void UpdateTaskAddDriver(int taskId, int driverId)
        {
            using (IDbConnection cnn = new SqlConnection(_conStr))
            {
                var p = new DynamicParameters();
                p.Add("@Id", taskId);
                p.Add("@DriverId", driverId);
                string sql = "UPDATE Tasks SET DriverId=@DriverId WHERE Id=@Id";
                cnn.Execute(sql, p);
            }
        }

    }
}