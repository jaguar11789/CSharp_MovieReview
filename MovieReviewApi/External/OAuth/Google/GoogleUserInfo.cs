using System.Text.Json.Serialization;

namespace MovieReviewApi.External.OAuth.Google
{
    public class GoogleUserInfo
    {
        [JsonPropertyName("sub")]
        public string? Sub           { get; set; } // 사용자 고유 식별자

        [JsonPropertyName("name")]
        public string? Name          { get; set; } // 사용자 전체 이름

        [JsonPropertyName("given_name")]
        public string? GivenName     { get; set; } // 사용자 이름

        [JsonPropertyName("family_name")]
        public string? FamilyName    { get; set; } // 사용자 성

        [JsonPropertyName("picture")]
        public string? Picture       { get; set; } // 사용자 프로필 사진

        [JsonPropertyName("email")]
        public string? Email         { get; set; } // 이메일 주소

        [JsonPropertyName("email.verified")]
        public bool    EmailVerified { get; set; } // 이메일 주소가 확인 되었는지 여부
    }
}
