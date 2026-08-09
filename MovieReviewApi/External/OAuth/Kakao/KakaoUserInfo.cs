using System.Text.Json.Serialization;

namespace MovieReviewApi.External.OAuth.Kakao
{
    public class KakaoUserInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("kakao_account")]
        public KakaoAccount? Account { get; set; }
    }

    public class KakaoAccount
    {
        [JsonPropertyName("profile")]
        public KakaoProfile? Profile { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class KakaoProfile
    {
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }
        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }
    }
}