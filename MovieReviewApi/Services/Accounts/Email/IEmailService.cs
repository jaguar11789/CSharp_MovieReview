namespace MovieReviewApi.Services.Accounts.Email
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationCode);
    }
}
