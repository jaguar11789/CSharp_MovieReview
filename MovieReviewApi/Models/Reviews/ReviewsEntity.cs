namespace MovieReviewApi.Models.Reviews
{
    public class ReviewsEntity
    {
        public long      Id         { get; set; }
        public long      UserId     { get; set; }
        public long      MovieId    { get; set; }
        public int       Rating     { get; set; }
        public string    Content    { get; set; } = string.Empty;

        public int       StatusCode { get; set; } = 0;
        public DateTime  CreatedAt  { get; set; }
        public DateTime? UpdatedAt  { get; set; }

        public ICollection<ReviewHistoryEntity> ReviewHistories { get; set; } = [];
    }
}
