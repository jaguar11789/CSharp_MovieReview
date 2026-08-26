using Azure;
using MovieReviewApi.DTOs.Admin;
using MovieReviewApi.Repositories.Admin;

namespace MovieReviewApi.Services.Admin
{
    public class AdminUserService(IAdminUserRepository adminUserRepository) : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository = adminUserRepository;

        public async Task<AdminUserListResponse> GetAllAsync(int page, int pageSize, string? keyword, int? statusCode, int? joinDays)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var (users, totalCount) = await _adminUserRepository.GetAllAsync(page, pageSize, keyword, statusCode, joinDays);

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new AdminUserListResponse
            {
                Items = users.Select(user => new AdminUserResponse
                {
                    Id         = user.Id,
                    UserId     = user.UserId,
                    Email      = user.Email,
                    Role       = user.Role,
                    CreatedAt  = user.CreatedAt,

                    StatusCode = user.StatusCode
                }).ToList(),

                Page       = page,
                PageSize   = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
