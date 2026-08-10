using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public interface IEmailVerificationRepository
    {
        Task<EmailVerificationEntity?> FindLastestAsync(long id, string email);

        void Add(EmailVerificationEntity emailVerificationEntity);

        Task SaveChangeAsync();
    }
}
