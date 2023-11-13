using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    public class QueuePayload
    {
        public string bucket { get; set; }
        public string filename { get; set; }

        public QueuePayload(string bucket, string filename)
        {
            this.bucket = bucket;
            this.filename = filename;
        }
    }
}
