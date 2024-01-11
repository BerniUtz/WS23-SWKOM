using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SWKOM_paperless.BusinessLogic;
using SWKOM_paperless.DAL;
using SWKOM_paperless.ServiceAgents;


namespace SWKOM_paperless.OCRWorker
{
    static class Program
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
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(config.GetConnectionString("DefaultConnection")).Options;
            var queueName = config.GetSection("Queue").ToString();
            var elasticSearchOptions = config.GetSection("ElasticSearch").Get<ElasticSearchOptions>();
            
            if(queueOptions == null || fileStorageOptions == null || queueName == null || elasticSearchOptions == null)

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
                    new OCRClient(),
                    new ElasticSearchService(
                        elasticSearchOptions.Endpoint,
                        elasticSearchOptions.Username,
                        elasticSearchOptions.Password,
                        elasticSearchOptions.IndexName
                    ),
                    new ApplicationDbContext(dbContextOptions)
                );

            client.startAsync();
        }
    }
}



