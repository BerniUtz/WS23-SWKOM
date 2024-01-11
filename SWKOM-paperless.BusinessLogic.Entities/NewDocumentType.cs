using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.Entities
{
    public class NewDocumentType
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Match { get; set; }
        public long MatchingAlgorithm { get; set; }
        public bool IsInsensitive { get; set; }
    }
}
