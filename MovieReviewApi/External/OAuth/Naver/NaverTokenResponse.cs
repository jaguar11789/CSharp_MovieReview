using System.Text.Json.Serialization;

namespace MovieReviewApi.External.OAuth.Naver
{
    public class NaverTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken {  get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public string? ExpiresIn { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription {  get; set; }
    }
}
