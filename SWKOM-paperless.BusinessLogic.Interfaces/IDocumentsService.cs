using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.BusinessLogic.Interfaces
{
    public interface IDocumentsService
    {
        public Task HandleUpload(List<IFormFile> documents);
        public Task<List<Document>> GetDocuments();
    }
}