namespace MovieReviewApi.DTOs.Reviews
{
    public class ReviewResponse
    {
        public long     Id        { get; set; }
        public long     MovieId   { get; set; }
        public string   UserId    { get; set; } = string.Empty;
        public int      Rating    { get; set; }
        public string   Content   { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
