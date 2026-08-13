namespace MovieReviewApi.Models.Accounts.User
{
    public class UserEntity
    {
        public long      Id            { get; set; }
        public string    UserId        { get; set; } = string.Empty;
        public string?   PasswordHash  { get; set; }
        public string    UserName      { get; set; } = string.Empty;
        public string?   Email         { get; set; }

        public string?   PhoneNumber   { get; set; }
        public string?   ZipCode       { get; set; }
        public string?   BaseAddress   { get; set; }
        public string?   DetailAddress { get; set; }
        public DateTime  CreatedAt     { get; set; }

        public int       StatusCode    { get; set; } = 100; // 기본값을 1로 설정 (예: 활성 상태)
        public DateTime  UpdatedAt     { get; set; } = DateTime.Now; // 기본값을 현재 시간으로 설정
        public DateTime? BirthDate     { get; set; } // 생년월일은 선택 사항이므로 nullable로 설정
        public string?   Gender        { get; set; }
        public string    Role          { get; set; } = "User"; // 기본값을 "User"로 설정

        public bool      EmailVerified { get; set; }

        public ICollection<UserHistoryEntity>       UserHistories      { get; set; } = [];
        public ICollection<UserSocialAccountEntity> UserSocialAccounts { get; set; } = [];
    }
}
