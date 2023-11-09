using System.Reflection;
using System.Text;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Moq;
using SWKOM_paperless.BusinessLogic;

namespace Tests;

[TestFixture]
public class MinioFileStorageServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _mockMinioClient = new Mock<IMinioClient>();
        _minioFileStorageService = new MinioFileStorageService("endpoint", "accessKey", "secretKey", BucketName);
        // inject the mocked minio client
        typeof(MinioFileStorageService)
            .GetField("_minioClient", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_minioFileStorageService, _mockMinioClient.Object);
    }

    private Mock<IMinioClient> _mockMinioClient = null!;
    private MinioFileStorageService _minioFileStorageService = null!;
    private const string BucketName = "test-bucket";

    [Test]
    public async Task UploadFileAsync_CallsPutObjectAsync()
    {
        // Arrange
        var fakeStream = new MemoryStream(Encoding.UTF8.GetBytes("fake file content"));
        const string filePath = "test-file.txt";
        _mockMinioClient.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), default))
            .Returns(Task.FromResult<PutObjectResponse>(null!));

        // Act
        await _minioFileStorageService.UploadFileAsync(fakeStream, filePath);

        // Assert
        _mockMinioClient.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), default), Times.Once);
    }

    [Test]
    public async Task GetFileAsync_CallsGetObjectAsync()
    {
        // Arrange
        const string filePath = "test-file.txt";
        _mockMinioClient.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), default))
            .Returns(Task.FromResult<ObjectStat>(null!));

        // Act
        await _minioFileStorageService.GetFileAsync(filePath);

        // Assert
        _mockMinioClient.Verify(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), default), Times.Once);
    }

    [Test]
    public async Task DeleteFileAsync_CallsRemoveObjectAsync()
    {
        // Arrange
        const string filePath = "test-file.txt";
        _mockMinioClient.Setup(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), default))
            .Returns(Task.CompletedTask);

        // Act
        await _minioFileStorageService.DeleteFileAsync(filePath);

        // Assert
        _mockMinioClient.Verify(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), default), Times.Once);
    }

    [Test]
    public async Task FileExistsAsync_CallsStatObjectAsync()
    {
        // Arrange
        const string filePath = "test-file.txt";
        _mockMinioClient.Setup(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), default))
            .Returns(Task.FromResult<ObjectStat>(null!));

        // Act
        await _minioFileStorageService.FileExistsAsync(filePath);

        // Assert
        _mockMinioClient.Verify(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), default), Times.Once);
    }
}