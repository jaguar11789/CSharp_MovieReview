using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Accounts.Auth;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> CheckUserIdAsync(string userId);

        Task<ResultResponse> RegisterAsync(RegisterRequest registerRequest);

        Task<LoginResponse?> LoginAsync(LoginRequest request);

        string CreateToken(UserEntity user);
    }
}
