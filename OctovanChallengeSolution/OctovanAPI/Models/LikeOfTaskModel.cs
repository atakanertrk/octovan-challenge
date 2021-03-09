using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Models
{
    /// <summary>
    /// This UserId is liked this TaskId
    /// </summary>
    public class LikeOfTaskModel
    {
        public int UserId { get; set; }
        public int TaskId { get; set; }
    }
}
