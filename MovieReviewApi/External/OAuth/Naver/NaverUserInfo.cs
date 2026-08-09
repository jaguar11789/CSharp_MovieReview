using System.Text.Json.Serialization;

public class NaverUserInfo
{
    [JsonPropertyName("resultcode")]
    public string?    ResultCode { get; set; }

    [JsonPropertyName("message")]
    public string?    Message    { get; set; }

    [JsonPropertyName("response")]
    public NaverUser? Response   { get; set; }
}

public class NaverUser
{
    [JsonPropertyName("id")]
    public string? Id           { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname     { get; set; }

    [JsonPropertyName("name")]
    public string? Name         { get; set; }

    [JsonPropertyName("email")]
    public string? Email        { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender       { get; set; }

    [JsonPropertyName("age")]
    public string? Age          { get; set; }

    [JsonPropertyName("birthday")]
    public string? Birthday     { get; set; }

    [JsonPropertyName("profile_image")]
    public string? ProfileImage { get; set; }

    [JsonPropertyName("birthyear")]
    public string? BirthYear    { get; set; }

    [JsonPropertyName("mobile")]
    public string? Mobile       { get; set; }
}