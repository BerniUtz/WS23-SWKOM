using System.Text.Json.Serialization;

namespace Mock_Server.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("date_joined")]
        public DateTime DateJoined { get; set; }

        [JsonPropertyName("is_staff")]
        public bool IsStaff { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("is_superuser")]
        public bool IsSuperuser { get; set; }

        [JsonPropertyName("groups")]
        public int[] Groups { get; set; }

        [JsonPropertyName("user_permissions")]
        public string[] UserPermissions { get; set; }

        [JsonPropertyName("inherited_permissions")]
        public string[] InheritedPermissions { get; set; }
    }
}