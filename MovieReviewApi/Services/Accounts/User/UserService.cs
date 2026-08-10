using Microsoft.AspNetCore.Identity;
using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;

namespace MovieReviewApi.Services.Accounts.User
{
    public class UserService(AppDbContext context, IUserRepository userRepository, IUserSocialAccountRepository userSocialAccountRepository, IUserHistoryRepository userHistoryRepository) : IUserService
    {
        private readonly AppDbContext                 _context                     = context;
        private readonly PasswordHasher<UserEntity>   _passwordHasher              = new();
        private readonly IUserRepository              _userRepository              = userRepository;
        private readonly IUserSocialAccountRepository _userSocialAccountRepository = userSocialAccountRepository;
        private readonly IUserHistoryRepository       _userHistoryRepository       = userHistoryRepository;

        // 내 정보 조회
        public async Task<UserResponse?> GetMyPageAsync(long id)
        {
            var user = await _userRepository.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var socialAccount = await _userSocialAccountRepository.FindByUserIdAsnc(id);

            return new UserResponse
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

                CreatedAt     = user.CreatedAt,
                Provider      = socialAccount?.Provider,
                EmailVerified = user.EmailVerified
            };
        }

        // 내 정보 수정
        public async Task<ResultResponse> ChangeUserInfoAsync(long id, UserUpdateRequest userUpdateRequest)
        {
            var user = await _userRepository.FindByIdAsync(id);

            if (user == null)
            {
                return new ResultResponse
                {
                    retVal = 900,
                    retMsg = "사용자 정보를 찾을 수 없습니다."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var emailChanged = !string.Equals(user.Email, userUpdateRequest.Email, StringComparison.OrdinalIgnoreCase);

                user.UserName      = userUpdateRequest.UserName;
                user.Email         = userUpdateRequest.Email;
                user.PhoneNumber   = userUpdateRequest.PhoneNumber;
                user.Gender        = userUpdateRequest.Gender;
                user.BirthDate     = userUpdateRequest.BirthDate;

                user.ZipCode       = userUpdateRequest.ZipCode;
                user.BaseAddress   = userUpdateRequest.BaseAddress;
                user.DetailAddress = userUpdateRequest.DetailAddress;
                user.UpdatedAt     = DateTime.Now;

                if (emailChanged)
                {
                    user.EmailVerified = false;
                }

                var history = new UserHistoryEntity
                {
                    User       = user,
                    ActionCode = 200,
                    StatusCode = user.StatusCode,
                    ChangedAt  = DateTime.Now,
                    Memo       = "회원 정보 수정"
                };

                _userHistoryRepository.Add(history);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                await transaction.RollbackAsync();

                throw;
            }

            return new ResultResponse
            {
                retVal = 0,
                retMsg = "회원 정보가 변경되었습니다."
            };
        }

        // 비밀번호 변경
        public async Task<ResultResponse> ChangePasswordAsync(long id, PasswordChangeRequest passwordChangeRequest)
        {
            var user = await _userRepository.FindByIdAsync(id);

            if (user == null)
            {
                return new ResultResponse
                {
                    retVal = 900,
                    retMsg = "사용자 정보를 찾을 수 없습니다."
                };
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, passwordChangeRequest.CurrentPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return new ResultResponse
                {
                    retVal = 800,
                    retMsg = "현재 비밀번호가 일치하지 않습니다."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, passwordChangeRequest.NewPassword);
                user.UpdatedAt    = DateTime.Now;

                var history = new UserHistoryEntity
                {
                    User       = user,
                    ActionCode = 300, // 비밀번호 변경
                    StatusCode = user.StatusCode,
                    ChangedAt  = DateTime.Now,
                    Memo       = "비밀번호 변경"
                };

                _userHistoryRepository.Add(history);

                await _userRepository.SaveChangeAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "비밀번호가 변경되었습니다."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}