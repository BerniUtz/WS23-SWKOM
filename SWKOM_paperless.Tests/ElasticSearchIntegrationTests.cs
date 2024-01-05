using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic;

namespace Tests;

[TestFixture]
public class ElasticSearchIntegrationTests
{
    private readonly ElasticSearchService _elasticSearchService = new ElasticSearchService();
    private const string IndexName = "documents";

    [SetUp]
    public void Setup()
    {
        // Arrange
        var document = new Document { Id = 1, Title = "Title", Content = "Content" };
        _elasticSearchService.AddDocumentAsync(document).Wait();
    }

    [Test]
    public async Task SearchByTitleAsync()
    {
        // Act: Index the document
        var result = await _elasticSearchService.SearchDocumentsAsync("Title", IndexName);

        // Assert: Ensure the document was found
        Assert.IsTrue(result.Count == 1);
        Assert.That(result[0].Id == 1);
        Assert.That(result[0].Title == "Title");
        Assert.That(result[0].Content == "Content");
    }
    
    [Test]
    public async Task SearchByContentAsync()
    {
        // Act: Index the document
        var result = await _elasticSearchService.SearchDocumentsAsync("Content", IndexName);

        // Assert: Ensure the document was found
        Assert.IsTrue(result.Count == 1);
        Assert.That(result[0].Id == 1);
        Assert.That(result[0].Title == "Title");
        Assert.That(result[0].Content == "Content");
    }
}