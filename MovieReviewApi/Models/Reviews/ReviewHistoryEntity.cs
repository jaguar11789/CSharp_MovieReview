namespace MovieReviewApi.Models.Reviews
{
    public class ReviewHistoryEntity
    {
        public long     Id         { get; set; }
        public long     ReviewId   { get; set; }
        public long     UserId     { get; set; }
        public int      ActionCode { get; set; }
        public int?     Rating     { get; set; }

        public string?  Content    { get; set; }
        public int      StatusCode { get; set; }
        public string?  Memo       { get; set; }
        public DateTime CreatedAt  { get; set; }

        public ReviewsEntity Reviews { get; set; } = null!;
    }
}
