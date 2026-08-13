namespace MovieReviewApi.Models.Accounts.User
{
    public class UserSocialAccountEntity
    {
        public long     Id              { get; set; }
        public long     UserId          { get; set; }
        public string   Provider        { get; set; } = string.Empty;
        public string   ProviderUserId  { get; set; } = string.Empty;
        public DateTime CreatedAt       { get; set; }

        public DateTime UpdatedAt       { get; set; }
        public string?  ProfileImageUrl { get; set; }

        public UserEntity User { get; set; } = null!;
    }
}
