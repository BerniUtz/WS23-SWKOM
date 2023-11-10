/*
 * Mock Server
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Org.OpenAPITools.Authentication;
using Org.OpenAPITools.Filters;
using Org.OpenAPITools.OpenApi;
using Org.OpenAPITools.Formatters;
using SWKOM_paperless.BusinessLogic;
using SWKOM_paperless.BusinessLogic.Interfaces;
using SWKOM_paperless.DAL;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Org.OpenAPITools
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            
            // ignore CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            
            // Add FileStorageService
            services.AddSingleton<IFileStorageService, MinioFileStorageService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var minioOptions = config.GetSection("MinIO").Get<MinIOOptions>();
                return new MinioFileStorageService(
                    minioOptions.Endpoint, 
                    minioOptions.AccessKey, 
                    minioOptions.SecretKey, 
                    minioOptions.BucketName
                    );
            });
            
            // Add QueueService
            services.AddSingleton<IQueueService, RabbitMQService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var rabbitMQOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>();
                return new RabbitMQService(
                    rabbitMQOptions.Hostname,
                    rabbitMQOptions.Username,
                    rabbitMQOptions.Password,
                    rabbitMQOptions.Port
                );
            });

            //Add QueueInitializer to ensure the queue is up and runnign
            services.AddSingleton<IHostedService, QueueInitializerService>();

            
            
            // DB Context zum Service hinzufügen
            // Connectionstring wird aus appsettings.json gelesen
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            // Add framework services.
            services
                // Don't need the full MVC stack for an API, see https://andrewlock.net/comparing-startup-between-the-asp-net-core-3-templates/
                .AddControllers(options => {
                    options.InputFormatters.Insert(0, new InputFormatterStream());
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                });
            services
                .AddSwaggerGen(c =>
                {
                    c.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
                    
                    c.SwaggerDoc("1.0", new OpenApiInfo
                    {
                        Title = "Paperless API Server",
                        Description = "",
                        TermsOfService = new Uri("https://github.com/openapitools/openapi-generator"),
                        Contact = new OpenApiContact
                        {
                            Name = "OpenAPI-Generator Contributors",
                            Url = new Uri("https://github.com/openapitools/openapi-generator"),
                            Email = ""
                        },
                        License = new OpenApiLicense
                        {
                            Name = "NoLicense",
                            Url = new Uri("http://localhost")
                        },
                        Version = "1.0",
                    });
                    c.CustomSchemaIds(type => type.FriendlyId(true));
                    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly().GetName().Name}.xml");

                    // Include DataAnnotation attributes on Controller Action parameters as OpenAPI validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });
            
            services
                .AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "openapi/{documentName}/openapi.json";
                })
                .UseSwaggerUI(c =>
                {
                    // set route prefix to openapi, e.g. http://localhost:8080/openapi/index.html
                    c.RoutePrefix = "openapi";
                    //TODO: Either use the SwaggerGen generated OpenAPI contract (generated from C# classes)
                    c.SwaggerEndpoint("/openapi/1.0/openapi.json", "Mock Server");

                    //TODO: Or alternatively use the original OpenAPI contract that's included in the static files
                    // c.SwaggerEndpoint("/openapi-original.json", "Mock Server Original");
                });
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
