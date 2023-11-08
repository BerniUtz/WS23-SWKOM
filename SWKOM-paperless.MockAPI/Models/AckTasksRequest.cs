using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public class AckTasksRequest
{
    [JsonPropertyName("tasks")]
    public IEnumerable<int> Tasks { get; set; }
}