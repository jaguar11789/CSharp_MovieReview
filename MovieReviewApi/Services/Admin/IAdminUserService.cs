using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Admin;

namespace MovieReviewApi.Services.Admin
{
    public interface IAdminUserService
    {
        Task<AdminUserListResponse> GetAllAsync(int page, int pageSize, string? keyword, int? statusCode, int? joinDays);

        Task<AdminUserResponse?> GetByIdAsync(long id);

        Task<ResultResponse> UpdateStatusAsync(long userId, int statusCode, string? memo); 
    }
}
