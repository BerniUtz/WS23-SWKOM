using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class ViewsListResponse : ListResponse<SavedView>
{
    [JsonPropertyName("all")]
    public int[] All { get; set; }
}