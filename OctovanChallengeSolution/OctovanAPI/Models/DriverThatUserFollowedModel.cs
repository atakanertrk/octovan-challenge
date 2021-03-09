using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Models
{
    /// <summary>
    /// This model represents this UserId is following this DriverId
    /// </summary>
    public class DriverThatUserFollowedModel
    {
        public int UserId { get; set; }
        public int DriverId { get; set; }
    }
}
