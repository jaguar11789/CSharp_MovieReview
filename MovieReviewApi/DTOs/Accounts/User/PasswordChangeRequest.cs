namespace MovieReviewApi.DTOs.Accounts.User
{
    public class PasswordChangeRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
