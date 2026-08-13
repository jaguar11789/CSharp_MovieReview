namespace MovieReviewApi.Models.Accounts.User
{
    public class UserHistoryEntity
    {
        public long     Id         { get; set; }
        public long     UserId     { get; set; }
        public int      ActionCode { get; set; }
        public int      StatusCode { get; set; }
        public string?  Memo       { get; set; }
        public DateTime ChangedAt  { get; set; }

        public UserEntity User { get; set; } = null!;
    }
}
