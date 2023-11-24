﻿using FluentValidation;
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
        
        public OCRService(IFileStorageService fileStorage, IQueueService queueService, string queue, IOCRClient ocrWorker)
        {
            _fileStorage = fileStorage;
            _queueService = queueService;
            _queue = queue;
            _ocrWorker = ocrWorker;
        }

        public async void startAsync()
        {
            //TODO create subscription functionality in IQueueService
            await _queueService.EnsureQueueExistsAsync(_queue);
            Console.WriteLine($"Queue {_queue} exists");

            var payload = readFromQueue();
            Stream pdfStream = await getPDFFileStream(payload.Result.filename);

            Console.Write($"{payload.Result.filename}: {_ocrWorker.OcrPdf(pdfStream)}");

            //TODO Save result in Database and ElasticSearch
        }


        private async Task<QueuePayload> readFromQueue()
        {
            //reading from queue
            QueuePayload messageBody = await _queueService.DequeueAsync<QueuePayload>(_queue);
            
            // check if there is a message in the queue
            if (messageBody == null)
            {
                // TODO: Rather than throwing an exception, the worker should wait for a message to be enqueued.
                throw new Exception("No message in Queue");
            }

            // validating messageBody
            var validator = new QueuePayloadValidator();
            var validationResult = validator.Validate(messageBody);
            if (!validationResult.IsValid)
            {
                throw new Exception("Invalid Messagebody");
            }

            return messageBody;
        }

        private async Task<Stream> getPDFFileStream(string fileName)
        {
            //getFile from Minio
            return await _fileStorage.GetFileAsync(fileName);

        }
    }
}