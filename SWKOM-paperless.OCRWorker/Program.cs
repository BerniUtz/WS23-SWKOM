using Microsoft.Extensions.Configuration;
using Minio;
using SWKOM_paperless.BusinessLogic;
using SWKOM_paperless.OCRWorker;
using SWKOM_paperless.ServiceAgents;
using SWKOM_paperless.ServiceAgents.Interfaces;
using System.Configuration;

namespace SWKOM_paperless.OCRWorker
{
    class Progamm
    {
        static void Main()
        {
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("OCRWorkerSettings.json")
            .Build();


            var queueOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>();
            var fileStorageOptions = config.GetSection("MinIO").Get<MinIOOptions>();
            var queueName = config.GetSection("Queue").ToString();

            OCRService client = new OCRService(
                new MinioFileStorageService(
                        fileStorageOptions.Endpoint,
                        fileStorageOptions.AccessKey,
                        fileStorageOptions.SecretKey,
                        fileStorageOptions.BucketName
                        ),
                new RabbitMQService(
                        queueOptions.Hostname,
                        queueOptions.Username,
                        queueOptions.Password,
                        queueOptions.Port
                    ),
                    queueName,
                    new OCRClient()
                );

            client.startAsync();
        }
    }
}



