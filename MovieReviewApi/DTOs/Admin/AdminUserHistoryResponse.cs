namespace MovieReviewApi.DTOs.Admin
{
    public class AdminUserHistoryResponse
    {
        public long     Id         { get; set; }
        public int      ActionCode { get; set; }
        public int      StatusCode { get; set; }
        public string?  Memo       { get; set; }
        public DateTime ChangedAt  { get; set; }
    }
}
