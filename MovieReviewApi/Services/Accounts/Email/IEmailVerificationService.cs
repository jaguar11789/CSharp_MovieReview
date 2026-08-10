using MovieReviewApi.Common.Responses;

namespace MovieReviewApi.Services.Accounts.Email
{
    public interface IEmailVerificationService
    {
        Task<ResultResponse> SendVerificationCodeAsync(long id);
        Task<ResultResponse> VerifyEmailAsync(long id, string email, string verificationCode);
    }
}
