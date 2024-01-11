using Microsoft.AspNetCore.Http;
using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic;

public class DocumentsService : IDocumentsService
{
    private readonly IFileStorageService _fileStorageService;

    private readonly IQueueService _queueService;

    public DocumentsService(IFileStorageService fileStorageService, IQueueService queueService)
    {
        _fileStorageService = fileStorageService;
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

            var message = new QueuePayload("BUCKET_PLACEHOLDER", fileName);
            await _queueService.EnqueueAsync("ocr_queue", message);
            
            // TODO: Add document to database
        }
    }
}

public class DocumentsServiceNoDocumentsException : ArgumentNullException
{
};