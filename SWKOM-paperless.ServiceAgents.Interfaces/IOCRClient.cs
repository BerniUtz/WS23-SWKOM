using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.ServiceAgents.Interfaces
{
    public interface IOCRClient
    {
        string OcrPdf(Stream pdf);
    }
}
