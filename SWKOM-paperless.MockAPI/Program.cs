using System.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Mock_Server.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.ResponseHeaders.Add("Content-Type");
                logging.ResponseHeaders.Add("current-route");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.MediaTypeOptions.AddText("application/json");
            });
        }

        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<DocumentType, NewDocumentType>().ReverseMap();
            cfg.CreateMap<Tag, NewTag>().ReverseMap();
            cfg.CreateMap<Correspondent, NewCorrespondent>().ReverseMap();
        });

        var app = builder.Build();

        app.UseCors();

        // app.Use((context, next) =>
        // {
        //     context.Request.EnableBuffering();
        //     var bodyAsText = new System.IO.StreamReader(context.Request.Body).ReadToEndAsync().Result;
        //     System.Console.WriteLine(bodyAsText);
        //     context.Request.Body.Position = 0;
        //     return next();
        // });

        app.UseWebSockets();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapFallbackToFile("/index.html");
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseHttpLogging();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouteDebugger();
        }

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }

    private async System.Threading.Tasks.Task FallbackMiddlewareHandler(HttpContext context, Func<System.Threading.Tasks.Task> next)
    {
        Debugger.Break();
        // // 404 - no match
        // // if (string.IsNullOrEmpty(ServerConfig.FolderNotFoundFallbackPath))
        // // {
        // //     await Status404Page(context);
        // //     return;
        // // }

        // // 404  - SPA fall through middleware - for SPA apps should fallback to index.html
        // var path = context.Request.Path;
        // if (string.IsNullOrEmpty(Path.GetExtension(path)))
        // {
        //     var file = Path.Combine("/wwwroot",
        //         ServerConfig.FolderNotFoundFallbackPath.Trim('/', '\\'));
        //     var fi = new FileInfo(file);
        //     if (fi.Exists)
        //     {
        //         if (!context.Response.HasStarted)
        //         {
        //             context.Response.ContentType = "text/html";
        //             context.Response.StatusCode = 200;
        //         }

        //         await context.Response.SendFileAsync(new PhysicalFileInfo(fi));
        //         await context.Response.CompleteAsync();
        //     }
        //     else
        //     {
        //         await Status404Page(context, isFallback: true);
        //     }
        // }

    }
}