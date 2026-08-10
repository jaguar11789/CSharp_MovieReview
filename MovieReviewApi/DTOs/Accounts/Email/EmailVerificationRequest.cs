namespace MovieReviewApi.DTOs.Accounts.Email
{
    public class EmailVerificationRequest
    {
        public string Email            { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}
