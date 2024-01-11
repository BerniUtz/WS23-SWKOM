using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic.EntityValidators;
using SWKOM_paperless.BusinessLogic.Interfaces;
using SWKOM_paperless.ServiceAgents.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SWKOM_paperless.OCRWorker
{
    public class OCRService
    {
        private IFileStorageService _fileStorage;
        private IQueueService _queueService;
        private IOCRClient _ocrWorker;
        private readonly string _queue;

        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);
        
        public OCRService(IFileStorageService fileStorage, IQueueService queueService, string queue, IOCRClient ocrWorker)
        {
            _fileStorage = fileStorage;
            _queueService = queueService;
            _queue = queue;
            _ocrWorker = ocrWorker;
        }

        public async Task startAsync()
        {
            Console.WriteLine("Subscribe to queue");
            await _queueService.EnsureQueueExistsAsync(_queue);
            Console.WriteLine($"Queue {_queue} exists");

            _queueService.Subscribe<QueuePayload>(_queue, HandleMessage);
            
            _exitEvent.WaitOne();
        }

        private async Task<Stream> getPDFFileStream(string fileName)
        {
            //getFile from Minio
            return await _fileStorage.GetFileAsync(fileName);

        }

        private async void HandleMessage(QueuePayload message)
        {
            Console.WriteLine("Handle message");
            var validator = new QueuePayloadValidator();
            var validationResult = validator.Validate(message);
            if (!validationResult.IsValid)
            {
                throw new Exception("Invalid Messagebody");
            }

            Console.Write($"retrieved payload {message.filename}");

            Stream pdfStream = await getPDFFileStream(message.filename);

            string pdfContent = _ocrWorker.OcrPdf(pdfStream);

        }


        public void Stop()
        {
            _exitEvent.Set();
        }
    }
}
