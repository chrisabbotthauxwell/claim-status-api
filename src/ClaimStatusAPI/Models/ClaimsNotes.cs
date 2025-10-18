using System.Text.Json.Serialization;

namespace ClaimStatusAPI.Models
{
    public class ClaimsNotes
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("notes")]
        public ClaimNote[] Notes { get; set; } = System.Array.Empty<ClaimNote>();
    }
}
