using OctovanAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.ModelsDTO
{
    public class DetailedInformationOfTask
    {
        public int TaskId { get; set; }
        public DetailedDriver Driver { get; set; }
        public DetailedUser User { get; set; }
        public DetailedTask Task { get; set; }
    }
}
