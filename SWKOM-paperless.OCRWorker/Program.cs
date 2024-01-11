using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minio;
using SWKOM_paperless.BusinessLogic;
using SWKOM_paperless.OCRWorker;
using SWKOM_paperless.ServiceAgents;
using SWKOM_paperless.ServiceAgents.Interfaces;
using SWKOM_paperless.DAL;
using System.Configuration;



namespace SWKOM_paperless.OCRWorker
{
    class Progamm
    {
        static async Task Main()
        {
            // check if the Environment Variable ENVIRONMENT is set to docker
            // if so, use the OCRWorkerSettings.docker.json file
            // otherwise use the OCRWorkerSettings.json file
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var settingsFile = environment == "docker" ? "OCRWorkerSettings.docker.json" : "OCRWorkerSettings.json";
            
            int maxAttempts = 3;
            int currentAttempt = 0;

            while(currentAttempt < maxAttempts)
            {
                Console.WriteLine("OCRWorker starts");
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFile, optional: false, reloadOnChange: true)
                .Build();
                try
                {
                
                    var queueOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>();
                    var fileStorageOptions = config.GetSection("MinIO").Get<MinIOOptions>();
                    var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(config.GetConnectionString("DefaultConnection")).Options;
                    var elasticSearchOptions = config.GetSection("ElasticSearch").Get<ElasticSearchOptions>();
                    var queueName = config["Queue"];
                    
                    if(queueOptions == null || fileStorageOptions == null || queueName == null || dbContextOptions == null || elasticSearchOptions == null)
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
                    Console.WriteLine("OCRService starts");
                    await client.startAsync();
                    
                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        client.Stop();
                        eventArgs.Cancel = true; // Prevent the application from exiting immediately
                    };

                    // Keep the application running
                    while (true)
                    {
                        await Task.Delay(1000); 
                    }
                }
                catch(Exception e)
                {
                    currentAttempt++;
                    Console.WriteLine($"OCRService could not be started: {e.Message}");

                    Console.WriteLine($"Retrying in {5 * currentAttempt} Seconds...");
                    await Task.Delay((5 * currentAttempt)*1000);

                }
            }
            Console.WriteLine($"OCRService could not be started after {maxAttempts} attempts. Exiting...");
        }
    }
}



