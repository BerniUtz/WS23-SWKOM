using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class DocumentType
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("match")]
    public string Match { get; set; }

    [JsonPropertyName("matching_algorithm")]
    public long MatchingAlgorithm { get; set; }

    [JsonPropertyName("is_insensitive")]
    public bool IsInsensitive { get; set; }

    [JsonPropertyName("document_count")]
    public long DocumentCount { get; set; }
}

public partial class NewDocumentType
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("match")]
    public string Match { get; set; }

    [JsonPropertyName("matching_algorithm")]
    public long MatchingAlgorithm { get; set; }

    [JsonPropertyName("is_insensitive")]
    public bool IsInsensitive { get; set; }
}