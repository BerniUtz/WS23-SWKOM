
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Mock_Server.Models;

public class DocumentsFilterModel : GenericFilterModel
{
    [FromQuery(Name = "query")]
    public string? Query { get; set; }

    [FromQuery(Name = "tags__id__all")]
    [JsonConverter(typeof(IntArrayConverter))]
    public uint[]? TagsIds { get; set; }

    [FromQuery(Name = "document_type__id")]
    public uint? DocTypeId { get; set; }

    [FromQuery(Name = "correspondent__id")]
    public uint? CorrespondentId { get; set; }

    [FromQuery(Name = "truncate_content")]
    public bool? TruncateContent { get; set; }
}

public class GenericFilterModel
{
    public uint? Page { get; set; }

    [FromQuery(Name = "page_size")]
    public uint? PageSize { get; set; }

    [FromQuery(Name = "ordering")]
    public string? Ordering { get; set; }
}
