using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    public class Document
    {
      
        public int Id { get; set; }
        public int? Correspondent { get; set; }
        public int? DocumentType { get; set; }
        public int? StoragePath { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<int> Tags { get; set; }
        /*
        public DateTime Created { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Added { get; set; }
        public string ArchiveSerialNumber { get; set; }
        public string OriginalFileName { get; set; }
        public string ArchivedFileName { get; set; }
        */
    }
}
