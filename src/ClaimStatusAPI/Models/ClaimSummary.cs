using System.Text.Json.Serialization;

namespace ClaimStatusAPI.Models
{
    public class ClaimSummary
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("customerSummary")]
        public string CustomerSummary { get; set; } = string.Empty;

        [JsonPropertyName("adjusterSummary")]
        public string AdjusterSummary { get; set; } = string.Empty;

        [JsonPropertyName("nextStep")]
        public string NextStep { get; set; } = string.Empty;
    }
}
