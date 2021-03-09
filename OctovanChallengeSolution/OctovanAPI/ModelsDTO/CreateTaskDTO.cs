using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.ModelsDTO
{
    public class CreateTaskDTO
    {
        public string Description { get; set; }
        public int UserId { get; set; }
    }
}
