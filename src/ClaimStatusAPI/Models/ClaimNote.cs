using System;
using System.Text.Json.Serialization;

namespace ClaimStatusAPI.Models
{
    public class ClaimNote
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }

        [JsonPropertyName("attachments")]
        public string[]? Attachments { get; set; }

        [JsonPropertyName("actions")]
        public string[]? Actions { get; set; }

        [JsonPropertyName("followUp")]
        public string? FollowUp { get; set; }
    }
}
