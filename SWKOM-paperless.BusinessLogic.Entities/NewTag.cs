using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    internal class NewTag
    { 
        public string Name { get; set; }
        public string Color { get; set; }
        public string Match { get; set; }
        public long MatchingAlgorithm { get; set; }
        public bool IsInsensitive { get; set; }
        public bool IsInboxTag { get; set; }

    }
}
