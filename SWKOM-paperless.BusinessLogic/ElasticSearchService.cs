using Elasticsearch.Net;
using log4net;
using Nest;
using SWKOM_paperless.BusinessLogic.Entities;
using SWKOM_paperless.BusinessLogic.Interfaces;

namespace SWKOM_paperless.BusinessLogic;

public class ElasticSearchService : IElasticSearchLogic
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(ElasticSearchService));
    private readonly IElasticClient _elasticClient;
    
    public ElasticSearchService()
    {
        var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
            .ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true)
            .DefaultIndex("documents")
            .BasicAuthentication("elastic", "password"); // TODO: Move to config file
        _elasticClient = new ElasticClient(settings);
    }
    public ElasticSearchService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }
    
    public async Task AddDocumentAsync(Document document)
    {
        var indexResponse = await _elasticClient.IndexAsync(document, idx => idx
                .Index("documents")
                .Id(document.Id) // Specify the document ID
                .Refresh(Refresh.WaitFor) // Specify refresh behavior after indexing
        );

        if (!indexResponse.IsValid)
        {
            _logger.Error($"Failed to index document: {indexResponse.DebugInformation}");
        }
        else
        {
            _logger.Info($"Document indexed: {indexResponse.Id}");
        }
    }
    public async Task<List<Document>> SearchDocumentsAsync(string query, string indexName)
    {
        var response = await _elasticClient.SearchAsync<Document>(s => s
            .Index(indexName)
            .Query(q => q
                    .Match(m => m
                        .Field(f => f.Title)
                        .Query(query)
                    ) || q
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query(query)
                    )
            )
        );

        if (response.IsValid)
        {
            _logger.Info($"Found {response.Documents.Count} documents for query: {query}");
            return response.Documents.ToList();
        }
        else if (response.ServerError != null)
        {
            _logger.Error($"Server error: {response.ServerError.Error}");
        }
        else
        {
            _logger.Info($"No documents found for query: {query}");
        }

        return new List<Document>();
    }
    
    public async Task DeleteDocumentAsync(int id)
    {
        var deleteResponse = await _elasticClient.DeleteAsync<Document>(id, idx => idx
            .Index("documents")
            .Refresh(Refresh.WaitFor)
        );

        if (!deleteResponse.IsValid)
        {
            _logger.Error($"Failed to delete document: {deleteResponse.DebugInformation}");
        }
        else
        {
            _logger.Info($"Document deleted: {deleteResponse.Id}");
        }
    }

}