using MovieReviewApi.DTOs.Admin;

namespace MovieReviewApi.Services.Admin
{
    public interface IAdminUserService
    {
        Task<AdminUserListResponse> GetAllAsync(int page, int pageSize, string? keyword, int? statusCode, int? joinDays);
    }
}
