using Microsoft.AspNetCore.Http;
using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic.Interfaces;
using SWKOM_paperless.DAL;

namespace SWKOM_paperless.BusinessLogic;

public class DocumentsService : IDocumentsService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly DocumentRepository _documentRepository;
    private readonly IQueueService _queueService;

    public DocumentsService(IFileStorageService fileStorageService, IQueueService queueService, ApplicationDbContext dbContext)
    {
        _fileStorageService = fileStorageService;
        _documentRepository = new DocumentRepository(dbContext);
        _queueService = queueService;
    }

    public async Task HandleUpload(List<IFormFile> documents)
    {
        if (documents == null || documents.Count == 0) throw new DocumentsServiceNoDocumentsException();

        foreach (var document in documents)
        {
            var guid = Guid.NewGuid();
            var fileName = $"{guid}_{document.FileName}";
            
            await _fileStorageService.UploadFileAsync(document.OpenReadStream(), fileName);
            // TODO: Add document to database
            var newDocument = new Document()
            {
                Title = fileName,
                Content = document.OpenReadStream().ToString() ?? "",
            };
            
            _documentRepository.AddDocument(newDocument);

            var message = new QueuePayload(newDocument.Id, "BUCKET_PLACEHOLDER", fileName);
            await _queueService.EnqueueAsync("ocr_queue", message);
            
            
        }
    }
}

public class DocumentsServiceNoDocumentsException : ArgumentNullException
{
};