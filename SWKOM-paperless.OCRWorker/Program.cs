using Microsoft.Extensions.Configuration;
using SWKOM_paperless.BusinessLogic;
using SWKOM_paperless.ServiceAgents;

namespace SWKOM_paperless.OCRWorker
{
    class Progamm
    {
        static void Main()
        {
            // check if the Environment Variable ENVIRONMENT is set to docker
            // if so, use the OCRWorkerSettings.docker.json file
            // otherwise use the OCRWorkerSettings.json file
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var settingsFile = environment == "docker" ? "OCRWorkerSettings.docker.json" : "OCRWorkerSettings.json";
            
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
            .Build();
            
            var queueOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>();
            var fileStorageOptions = config.GetSection("MinIO").Get<MinIOOptions>();
            var queueName = config.GetSection("Queue").ToString();
            
            if(queueOptions == null || fileStorageOptions == null || queueName == null)
                throw new Exception("Failed to read configuration file.");

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



