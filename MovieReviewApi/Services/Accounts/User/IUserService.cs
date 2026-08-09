using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Accounts.User;

namespace MovieReviewApi.Services.Accounts.User
{
    public interface IUserService
    {
        Task<UserResponse?> GetMyPageAsync(long id);

        Task<ResultResponse> ChangePasswordAsync(long id, PasswordChangeRequest passwordChangeRequest);

        Task<ResultResponse> ChangeUserInfoAsync(long id, UserUpdateRequest userUpdateRequest);
    }
}
