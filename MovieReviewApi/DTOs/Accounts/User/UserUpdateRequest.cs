namespace MovieReviewApi.DTOs.Accounts.User
{
    public class UserUpdateRequest
    {
        public string?   UserName      { get; set; }
        public string?   Email         { get; set; }
        public string?   PhoneNumber   { get; set; }
        public string?   Gender        { get; set; }
        public DateTime? BirthDate     { get; set; }

        public string?   ZipCode       { get; set; }
        public string?   BaseAddress   { get; set; }
        public string?   DetailAddress { get; set; }
    }
}
