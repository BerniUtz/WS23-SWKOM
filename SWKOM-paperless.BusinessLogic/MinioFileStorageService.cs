using Minio;
using Minio.DataModel.Args;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic;

public class MinioFileStorageService: IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    
    public MinioFileStorageService(string endpoint, string accessKey, string secretKey, string bucketName)
    {
        // Initialize the MinIO client with the provided credentials and bucket.
        _minioClient = new MinioClient().WithEndpoint(endpoint).WithCredentials(accessKey, secretKey).Build();
        _bucketName = bucketName;
    }
    
    public async Task UploadFileAsync(Stream fileStream, string filePath)
    {
        // Upload the file to the configured bucket.
        var putObjectArgs = new PutObjectArgs().WithBucket(_bucketName).WithStreamData(fileStream).WithObject(filePath);
        await _minioClient.PutObjectAsync(putObjectArgs);
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        // Retrieve the file from the configured bucket.
        var memoryStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs().WithBucket(_bucketName).WithObject(filePath)
            .WithCallbackStream((stream) => stream.CopyTo(memoryStream));
        await _minioClient.GetObjectAsync(getObjectArgs);
        
        // Reset the stream position to the beginning. (This is required for the stream to be read.)
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteFileAsync(string filePath)
    {
        // Delete the file from the configured bucket.
        var removeObjectArgs = new RemoveObjectArgs().WithBucket(_bucketName).WithObject(filePath);
        await _minioClient.RemoveObjectAsync(removeObjectArgs);
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        // Check if the file exists in the configured bucket.
        var statObjectArgs = new StatObjectArgs().WithBucket(_bucketName).WithObject(filePath);
        var result = await _minioClient.StatObjectAsync(statObjectArgs);
        
        // If the file exists, the result will be non-null.
        return result != null;
    }
}

public class MinIOOptions
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
}