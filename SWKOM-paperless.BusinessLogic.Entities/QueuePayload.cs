using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    public class QueuePayload
    {
        public string Bucket { get; set; }
        public string Filename { get; set; }

        public QueuePayload(string bucket, string filename)
        {
            this.Bucket = bucket;
            this.Filename = filename;
        }
    }
}
