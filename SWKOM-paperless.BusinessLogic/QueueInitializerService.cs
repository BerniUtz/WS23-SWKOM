using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic
{
    public class QueueInitializerService : IHostedService
    {
        private readonly IQueueService _queueService;

        public QueueInitializerService(IQueueService queueService)
        {
            _queueService = queueService;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("checking Queue");
            try 
            {
                await _queueService.EnsureQueueExistsAsync("ocr_queue");
                Console.WriteLine("Queue is running");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing the queue: {ex.Message}");
            }
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
