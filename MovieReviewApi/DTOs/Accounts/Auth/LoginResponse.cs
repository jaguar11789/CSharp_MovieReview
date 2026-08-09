using MovieReviewApi.DTOs.Accounts.User;

namespace MovieReviewApi.DTOs.Accounts.Auth
{
    public class LoginResponse
    {
        public string       Token { get; set; } = string.Empty;
        public UserResponse User  { get; set; } = null!;
    }
}
