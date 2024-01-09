using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SWKOM_paperless.BusinessLogic.Interfaces
{
    public interface IDocumentsService
    {
        public Task HandleUpload(List<IFormFile> documents);
    }
}