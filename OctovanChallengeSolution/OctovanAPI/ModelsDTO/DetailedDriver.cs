using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.ModelsDTO
{
    public class DetailedDriver
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public bool Followed { get; set; } // is requesting user following this driver 
    }
}
