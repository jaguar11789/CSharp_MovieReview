using System.Text.Json.Serialization;

namespace MovieReviewApi.External.OAuth.Kakao
{
    public class KakaoTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }
}
