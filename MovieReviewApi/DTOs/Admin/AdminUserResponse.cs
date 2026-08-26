namespace MovieReviewApi.DTOs.Admin
{
    public class AdminUserResponse
    {
        public long     Id            { get; set; }
        public string   UserId        { get; set; } = string.Empty;
        public string   UserName      { get; set; } = string.Empty;
        public string   Email         { get; set; } = string.Empty;
        public string   PhoneNumber   { get; set; } = string.Empty;

        public string   Gender        { get; set; } = string.Empty;
        public DateTime BirthDate     { get; set; }
        public string   ZipCode       { get; set; } = string.Empty;
        public string   BaseAddress   { get; set; } = string.Empty;
        public string   DetailAddress { get; set; } = string.Empty;

        public string   Role          { get; set; } = string.Empty;
        public int      StatusCode    { get; set; }
        public DateTime CreatedAt     { get; set; }
        public DateTime UpdatedAt     { get; set; }

        public List<AdminUserHistoryResponse> History { get; set; } = [];
    }
}
