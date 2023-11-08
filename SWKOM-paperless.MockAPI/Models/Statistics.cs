using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class Statistics
{
    [JsonPropertyName("documents_total")]
    public long DocumentsTotalCount { get; set; }

    [JsonPropertyName("documents_inbox")]
    public long DocumentsInboxCount { get; set; }

    [JsonPropertyName("inbox_tag")]
    public long InboxTagCount { get; set; }

    [JsonPropertyName("character_count")]
    public long CharacterCount { get; set; }

    [JsonPropertyName("document_file_type_counts")]
    public IList<DocumentFileTypeCount> DocumentFileTypeCounts { get; set; }
}

public class DocumentFileTypeCount
{
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }

    [JsonPropertyName("mime_type_count")]
    public long Count { get; set; }
}