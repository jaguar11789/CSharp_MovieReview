using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public class EmailVerificationRepository(AppDbContext context) : IEmailVerificationRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<EmailVerificationEntity?> FindLastestAsync(long id, string email)
        {
            return await _context.EmailVerifications.Where(x => x.UserId == id && x.Email == email)
                                                    .OrderByDescending(x => x.CreatedAt)
                                                    .FirstOrDefaultAsync();
        }

        public void Add(EmailVerificationEntity emailVerificationEntity)
        {
            _context.EmailVerifications.Add(emailVerificationEntity);
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
