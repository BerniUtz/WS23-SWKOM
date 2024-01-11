﻿using Microsoft.Extensions.Configuration;
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
        static async Task Main()
        {
            int maxAttempts = 3;
            int currentAttempt = 0;

            while(currentAttempt < maxAttempts)
            {
                Console.WriteLine("OCRWorker starts");
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("OCRWorkerSettings.json")
                .Build();
                
                var queueOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>();
                var fileStorageOptions = config.GetSection("MinIO").Get<MinIOOptions>();
                var queueName = config["Queue"];
                
                if(queueOptions == null || fileStorageOptions == null || queueName == null)
                    throw new Exception("Failed to read configuration file.");
                try
                {
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
                    Console.WriteLine("OCRService starts");
                    await client.startAsync();
                    Console.WriteLine("Press Ctrl+C to stop the service.");
                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        client.Stop();
                        eventArgs.Cancel = true; // Prevent the application from exiting immediately
                    };

                    // Keep the application running
                    while (true)
                    {
                        await Task.Delay(1000); // Add a small delay to avoid excessive CPU usage
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



