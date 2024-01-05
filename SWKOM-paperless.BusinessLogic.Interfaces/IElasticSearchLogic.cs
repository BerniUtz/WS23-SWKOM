using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.BusinessLogic.Interfaces
{
    public interface IElasticSearchLogic
    {
        public Task AddDocumentAsync(Document document);
        public Task<List<Document>> SearchDocumentsAsync(string query, string indexName);

    }
}