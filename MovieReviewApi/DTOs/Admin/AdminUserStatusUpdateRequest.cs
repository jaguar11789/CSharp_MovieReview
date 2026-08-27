namespace MovieReviewApi.DTOs.Admin
{
    public class AdminUserStatusUpdateRequest
    {
        public int     StatusCode { get; set; }
        public string? Memo       { get; set; }
    }
}
