using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Models
{
    /// <summary>
    /// Task created by UserId and DriverId is assigned for the task.
    /// CreatedAt represents passed seconds from 01.01.1970 (unix timestamp)
    /// Default value of DriverId is null in database (if DriverId equals 0 then Driver has not assigned to task yet)
    /// </summary>
    public class TaskModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int DriverId { get; set; }
        public int CreatedAt { get; set; }
    }
}
