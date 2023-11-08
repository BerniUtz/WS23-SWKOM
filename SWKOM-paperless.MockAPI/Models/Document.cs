using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class Document
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("correspondent")]
    public uint? Correspondent { get; set; }

    [JsonPropertyName("document_type")]
    public uint? DocumentType { get; set; }

    [JsonPropertyName("storage_path")]
    public uint? StoragePath { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("tags")]
    public uint[] Tags { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("created_date")]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; }

    [JsonPropertyName("added")]
    public DateTime Added { get; set; }

    [JsonPropertyName("archive_serial_number")]
    public string? ArchiveSerialNumber { get; set; }

    [JsonPropertyName("original_file_name")]
    public string OriginalFileName { get; set; }

    [JsonPropertyName("archived_file_name")]
    public string ArchivedFileName { get; set; }
}
