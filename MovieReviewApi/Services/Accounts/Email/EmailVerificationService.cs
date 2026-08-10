using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;
using MovieReviewApi.Repositories.User;
using System.Security.Cryptography;

namespace MovieReviewApi.Services.Accounts.Email
{
    public class EmailVerificationService(AppDbContext context, IUserRepository userRepository, IEmailService emailService,IEmailVerificationRepository emailVerificationRepository, IUserHistoryRepository userHistoryRepository) : IEmailVerificationService
    {
        private readonly AppDbContext                 _context                     = context;
        private readonly IUserRepository              _userRepository              = userRepository;
        private readonly IEmailService                _emailService                = emailService;
        private readonly IEmailVerificationRepository _emailVerificationRepository = emailVerificationRepository;
        private readonly IUserHistoryRepository       _userHistoryRepository       = userHistoryRepository;

        public async Task<ResultResponse> SendVerificationCodeAsync(long id)
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

            if (string.IsNullOrEmpty(user.Email))
            {
                return new ResultResponse
                {
                    retVal = 800,
                    retMsg = "등록된 이메일이 없습니다."
                };
            }

            if (user.EmailVerified)
            {
                return new ResultResponse
                {
                    retVal = 700,
                    retMsg = "이미 인증된 이메일입니다."
                };
            }

            var verificationCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            var now = DateTime.Now;

            var verification = new EmailVerificationEntity
            {
                UserId           = user.Id,
                Email            = user.Email,
                VerificationCode = verificationCode,
                CreatedAt        = now,
                ExpiresAt        = now.AddMinutes(5)
            };

            _emailVerificationRepository.Add(verification);

            await _emailVerificationRepository.SaveChangeAsync();
            await _emailService.SendVerificationEmailAsync(user.Email, verificationCode);

            return new ResultResponse
            {
                retVal = 0,
                retMsg = "인증번호가 이메일로 전송되었습니다."
            };
        }

        public async Task<ResultResponse> VerifyEmailAsync(long id, string email, string verificationCode)
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

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return new ResultResponse
                {
                    retVal = 800,
                    retMsg = "등록된 이메일이 없습니다."
                };
            }

            if (user.EmailVerified)
            {
                return new ResultResponse
                {
                    retVal = 600,
                    retMsg = "이메일 정보가 일치하지 않습니다."
                };
            }

            var verification = await _emailVerificationRepository.FindLastestAsync(user.Id, email);

            if (verification == null)
            {
                return new ResultResponse
                {
                    retVal = 500,
                    retMsg = "인증번호 요청 기록을 찾을 수 없습니다."
                };
            }
            
            if (verification.ExpiresAt < DateTime.Now)
            {
                return new ResultResponse
                {
                    retVal = 400,
                    retMsg = "인증번호가 만료되었습니다."
                };
            }

            if (verification.VerificationCode != verificationCode)
            {
                return new ResultResponse
                {
                    retVal = 300,
                    retMsg = "인증번호가 일치하지 않습니다."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                user.EmailVerified = true;
                user.UpdatedAt     = DateTime.Now;

                verification.VerifiedAt = DateTime.Now;

                var history = new UserHistoryEntity
                {
                    User       = user,
                    ActionCode = 300, // 이메일 인증
                    StatusCode = user.StatusCode,
                    ChangedAt  = DateTime.Now,
                    Memo       = "이메일 인증 완료"
                };

                _userHistoryRepository.Add(history);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "이메일 인증이 완료되었습니다."
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
