using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.Admin
{
    public interface IAdminUserRepository
    {
        Task<(List<UserEntity> Users, int TotalCount)> GetAllAsync(int page, int pageSize, string? keyword, int? statusCode, int? joinDays);

        Task<UserEntity?> GetByIdAsync(long id);
    }
}
