namespace MovieReviewApi.DTOs.Reviews
{
    public class CreateReviewRequest
    {
        public long   MovieId { get; set; }
        public int    Rating  { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
