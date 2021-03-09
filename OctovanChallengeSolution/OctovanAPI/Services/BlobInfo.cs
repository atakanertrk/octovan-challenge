using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OctovanAPI.Services
{
    public class BlobInfo
    {
        public Stream content { get; set; }
        public string contentType { get; set; }
        public BlobInfo(Stream content, string contentType)
        {
            this.content = content;
            this.contentType = contentType;
        }
    }
}
