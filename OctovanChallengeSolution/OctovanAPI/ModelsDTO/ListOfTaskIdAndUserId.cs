using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.ModelsDTO
{
    public class ListOfTaskIdAndUserId
    {
        public List<int> TaskIds { get; set; }
        public int UserId { get; set; }
    }
}
