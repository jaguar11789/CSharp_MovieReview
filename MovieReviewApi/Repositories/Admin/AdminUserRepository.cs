using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.Admin
{
    public class AdminUserRepository(AppDbContext context) : IAdminUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<(List<UserEntity> Users, int TotalCount)> GetAllAsync(int page, int pageSize, string? keyword, int? statusCode, int? joinDays)
        {
            var query = _context.Users.AsNoTracking()
                                      .AsQueryable();

            // 검색
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.UserId.Contains(keyword) || x.Email.Contains(keyword));
            }

            // 상태
            if (statusCode.HasValue)
            {
                query = query.Where(x => x.StatusCode == statusCode.Value);
            }

            // 가입 기간
            if (joinDays.HasValue)
            {
                var startDate = DateTime.Now.AddDays(-joinDays.Value);

                query = query.Where(x => x.CreatedAt >= startDate);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync();

            var users = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (users, totalCount);
        }

        public async Task<UserEntity?> GetByIdAsync(long id)
        {
            return await _context.Users.Include(x => x.UserHistories)
                                       .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
