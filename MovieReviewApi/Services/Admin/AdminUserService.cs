using Azure;
using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Admin;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.Admin;

namespace MovieReviewApi.Services.Admin
{
    public class AdminUserService(IAdminUserRepository adminUserRepository, AppDbContext context) : IAdminUserService
    {
        private readonly AppDbContext _context = context;

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

        public async Task<AdminUserResponse?> GetByIdAsync(long id)
        {
            var user = await _adminUserRepository.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return new AdminUserResponse
            {
                Id            = user.Id,
                UserId        = user.UserId,
                UserName      = user.UserName,
                Email         = user.Email,
                PhoneNumber   = user.PhoneNumber,

                Gender        = user.Gender,
                BirthDate     = user.BirthDate,
                ZipCode       = user.ZipCode,
                BaseAddress   = user.BaseAddress,
                DetailAddress = user.DetailAddress,

                Role          = user.Role,
                StatusCode    = user.StatusCode,
                CreatedAt     = user.CreatedAt,
                UpdatedAt     = user.UpdatedAt,
                EmailVerified = user.EmailVerified,

                History = user.UserHistories.OrderByDescending(x => x.ChangedAt)
                                            .Select(x => new AdminUserHistoryResponse
                                            {
                                                Id         = x.Id,
                                                ActionCode = x.ActionCode,
                                                StatusCode = x.StatusCode,
                                                Memo       = x.Memo,
                                                ChangedAt  = x.ChangedAt
                                            }).ToList()
            };
        }

        public async Task<ResultResponse> UpdateStatusAsync(long userId, int statusCode, string? memo)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _adminUserRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return new ResultResponse
                    {
                        retVal = 900,
                        retMsg = "존재하지 않는 회원입니다."
                    };
                }

                if (statusCode != 100 && statusCode != 990)
                {
                    return new ResultResponse
                    {
                        retVal = 400,
                        retMsg = "잘못된 회원 상태입니다."
                    };
                }

                if (user.StatusCode == statusCode)
                {
                    return new ResultResponse
                    {
                        retVal = 300,
                        retMsg = "현재 회원 상태와 동일합니다."
                    };
                }

                if (user.StatusCode == 999)
                {
                    return new ResultResponse
                    {
                        retVal = 400,
                        retMsg = "탈퇴한 회원의 상태는 변경할 수 없습니다."
                    };
                }

                user.StatusCode = statusCode;
                user.UpdatedAt  = DateTime.Now;

                user.UserHistories.Add(new UserHistoryEntity
                {
                    UserId     = user.Id,
                    ActionCode = statusCode,
                    StatusCode = statusCode,
                    Memo       = memo,
                    ChangedAt  = DateTime.Now
                });

                await _adminUserRepository.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "회원 상태가 변경되었습니다."
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
