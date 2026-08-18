namespace MovieReviewApi.DTOs.Reviews
{
    public class ReviewRequest
    {
        public int    Rating  { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
