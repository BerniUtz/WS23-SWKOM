using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Hosting;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic
{
    public class QueueInitializerService : IHostedService
    {
        private readonly IQueueService _queueService;
        private readonly ILog _logger;

        public QueueInitializerService(IQueueService queueService)
        {
            _logger = LogManager.GetLogger(typeof(QueueInitializerService));
            _queueService = queueService;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.InfoFormat("Checking Queue");
            try 
            {
                await _queueService.EnsureQueueExistsAsync("ocr_queue");
                _logger.InfoFormat("Queue is running");
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat($"Error initializing the queue: {ex.Message}");
            }
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
