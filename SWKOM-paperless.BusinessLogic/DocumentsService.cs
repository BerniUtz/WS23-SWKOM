using Microsoft.AspNetCore.Http;
using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic.Interfaces;
using SWKOM_paperless.DAL;

namespace SWKOM_paperless.BusinessLogic;

public class DocumentsService : IDocumentsService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IElasticSearchLogic _elasticSearchLogic;
    private readonly DocumentRepository _documentRepository;
    private readonly IQueueService _queueService;

    public DocumentsService(IFileStorageService fileStorageService, IQueueService queueService, DocumentRepository documentRepository, IElasticSearchLogic elasticSearchLogic)
    {
        _fileStorageService = fileStorageService;
        _documentRepository = documentRepository;
        _queueService = queueService;
        _elasticSearchLogic = elasticSearchLogic;
    }

    public async Task HandleUpload(List<IFormFile> documents)
    {
        if (documents == null || documents.Count == 0) throw new DocumentsServiceNoDocumentsException();

        foreach (var document in documents)
        {
            var guid = Guid.NewGuid();
            var fileName = $"{guid}_{document.FileName}";
            
            await _fileStorageService.UploadFileAsync(document.OpenReadStream(), fileName);
            
            var newDocument = new Document()
            {
                Title = fileName,
                Content = document.OpenReadStream().ToString() ?? "",
            };
            
            _documentRepository.AddDocument(newDocument);
            _elasticSearchLogic.AddDocumentAsync(newDocument).Wait();

            var message = new QueuePayload(newDocument.Id, "BUCKET_PLACEHOLDER", fileName);
            await _queueService.EnqueueAsync("ocr_queue", message);
            
            
        }
    }

    public Task<List<Document>> GetDocuments()
    {
        return Task.FromResult(_documentRepository.GetAllDocuments()?.ToList() ?? new List<Document>());
    }
}

public class DocumentsServiceNoDocumentsException : ArgumentNullException
{
};