using MovieReviewApi.DTOs.Accounts.Auth;

namespace MovieReviewApi.Services.Social
{
    public interface IGoogleAuthService
    {
        string GetLoginUrl(string returnUrl);

        Task<LoginResponse> LoginAsync(string code, string state);
    }
}
