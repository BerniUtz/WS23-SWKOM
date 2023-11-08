using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public class Task
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("task_id")]
    public Guid TaskId { get; set; }

    [JsonPropertyName("task_file_name")]
    public string TaskFileName { get; set; }

    [JsonPropertyName("date_created")]
    public DateTimeOffset DateCreated { get; set; }

    [JsonPropertyName("date_done")]
    public DateTimeOffset DateDone { get; set; }

    [JsonPropertyName("type"),
        JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskType Type { get; set; }

    [JsonPropertyName("status"),
     JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskState Status { get; set; }

    [JsonPropertyName("result")]
    public string Result { get; set; }

    [JsonPropertyName("acknowledged")]
    public bool Acknowledged { get; set; }

    [JsonPropertyName("related_document")]
    public string RelatedDocument { get; set; }
}

public enum TaskType
{
    file
}

public enum TaskState
{
    PENDING,
    STARTED,
    SUCCESS,
    FAILURE
}
