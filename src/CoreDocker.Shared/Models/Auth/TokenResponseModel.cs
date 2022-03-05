
using System.Text.Json.Serialization;

namespace CoreDocker.Shared.Models.Auth
{
    public class TokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}