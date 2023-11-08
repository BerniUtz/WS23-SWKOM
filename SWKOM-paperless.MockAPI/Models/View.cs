using System.Text.Json.Serialization;

namespace Mock_Server.Models;

public partial class SavedView
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("show_on_dashboard")]
    public bool ShowOnDashboard { get; set; }

    [JsonPropertyName("show_in_sidebar")]
    public bool ShowInSidebar { get; set; }

    [JsonPropertyName("sort_field")]
    public string SortField { get; set; }

    [JsonPropertyName("sort_reverse")]
    public bool SortReverse { get; set; }

    [JsonPropertyName("filter_rules")]
    public FilterRule[] FilterRules { get; set; }

    [JsonPropertyName("owner")]
    public User Owner { get; set; }

    [JsonPropertyName("user_can_change")]
    public bool UserCanChange { get; set; }
}
public partial class FilterRule
{
    [JsonPropertyName("rule_type")]
    public long RuleType { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}