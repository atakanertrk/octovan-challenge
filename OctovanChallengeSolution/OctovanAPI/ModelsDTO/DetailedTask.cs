using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.ModelsDTO
{
    public class DetailedTask
    {
        public int Id { get; set; }
        public int Owner { get; set; } // userid
        public int AssignedDriver { get; set; } // driverid
        public List<string> Images { get; set; }
        public int CreatedAt { get; set; }
        public bool Liked { get; set; } // did user like this task ( check it via requesting user )
    }
}
