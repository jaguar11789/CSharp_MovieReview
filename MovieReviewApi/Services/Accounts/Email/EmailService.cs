using System.Net;
using System.Net.Mail;

namespace MovieReviewApi.Services.Accounts.Email
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var smtpHost     = _configuration["Email:SmtpHost"];
            var smtpPort     = int.Parse(_configuration["Email:SmtpPort"]!);
            var smtpEmail    = _configuration["Email:Username"];
            var smtpPassword = _configuration["Email:Password"];

            using var message = new MailMessage();

            message.From = new MailAddress(smtpEmail!, "Movie Review");

            message.To.Add(email);

            message.Subject = "Movie Review 이메일 인증";
            message.Body = $"""
                            안녕하세요.

                            이메일 인증번호는 다음과 같습니다.

                            {verificationCode}

                            인증번호는 5분 동안 유효합니다.

                            감사합니다.
                            """;

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    smtpEmail, smtpPassword)
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
