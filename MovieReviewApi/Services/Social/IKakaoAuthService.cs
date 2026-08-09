using MovieReviewApi.DTOs.Accounts.Auth;

namespace MovieReviewApi.Services.Social
{
    public interface IKakaoAuthService
    {
        string GetLoginUrl(string returnUrl);

        Task<LoginResponse> LoginAsync(string code, string state);
    }
}
