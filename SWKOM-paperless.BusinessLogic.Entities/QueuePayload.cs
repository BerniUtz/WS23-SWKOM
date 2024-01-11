using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    public class QueuePayload
    {
        public int Id { get; set; }
        public string Bucket { get; set; }
        public string Filename { get; set; }

        public QueuePayload(int id, string bucket, string filename)
        {
            this.Id = id;
            this.Bucket = bucket;
            this.Filename = filename;
        }
    }
}
