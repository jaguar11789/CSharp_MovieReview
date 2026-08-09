namespace MovieReviewApi.DTOs.Accounts.User
{
    public class UserResponse
    {
        public long      Id            { get; set; }
        public string    UserId        { get; set; } = string.Empty;
        public string    UserName      { get; set; } = string.Empty;
        public string?   Email         { get; set; }
        public string?   PhoneNumber   { get; set; }

        public string?   ZipCode       { get; set; }
        public string?   BaseAddress   { get; set; }
        public string?   DetailAddress { get; set; }
        public DateTime  CreatedAt     { get; set; }
        public int       StatusCode    { get; set; }

        public DateTime? BirthDate     { get; set; } // 생년월일은 선택 사항이므로 nullable로 설정
        public string?   Gender        { get; set; }
        public DateTime  UpdatedAt     { get; set; }
        public string?   Provider      { get; set; }
    }
}
