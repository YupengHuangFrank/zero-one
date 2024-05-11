using System.Text.Json.Serialization;

namespace ResumeBuilder.Api.Authentication.Models
{
    public class ErrorResponseApi
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }
        [JsonPropertyName("error_description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; set; }
        [JsonPropertyName("error_uri")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorUri { get; set; }
    }
}
