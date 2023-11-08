using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class DocumentMetadata
{
    [JsonPropertyName("original_checksum")]
    public string OriginalChecksum { get; set; }

    [JsonPropertyName("original_size")]
    public long OriginalSize { get; set; }

    [JsonPropertyName("original_mime_type")]
    public string OriginalMimeType { get; set; }

    [JsonPropertyName("media_filename")]
    public string MediaFilename { get; set; }

    [JsonPropertyName("has_archive_version")]
    public bool HasArchiveVersion { get; set; }

    [JsonPropertyName("original_metadata")]
    public object[] OriginalMetadata { get; set; }

    [JsonPropertyName("archive_checksum")]
    public string ArchiveChecksum { get; set; }

    [JsonPropertyName("archive_media_filename")]
    public string ArchiveMediaFilename { get; set; }

    [JsonPropertyName("original_filename")]
    public string OriginalFilename { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("archive_size")]
    public long ArchiveSize { get; set; }

    [JsonPropertyName("archive_metadata")]
    public object[] ArchiveMetadata { get; set; }
}