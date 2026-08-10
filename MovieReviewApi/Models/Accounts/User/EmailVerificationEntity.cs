namespace MovieReviewApi.Models.Accounts.User
{
    public class EmailVerificationEntity
    {
        public long       Id               { get; set; }
        public long       UserId           { get; set; }
        public string     Email            { get; set; } = null!;
        public string     VerificationCode { get; set; } = null!;
        public DateTime   ExpiresAt        { get; set; }

        public DateTime?  VerifiedAt       { get; set; }
        public DateTime   CreatedAt        { get; set; }
        public UserEntity User             { get; set; } = null!;
    }
}
