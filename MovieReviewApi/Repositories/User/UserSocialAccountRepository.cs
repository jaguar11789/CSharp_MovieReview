using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public class UserSocialAccountRepository(AppDbContext context) : IUserSocialAccountRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<UserSocialAccountEntity?> FindByProviderUserIdAsync(string provider, string providerUserId)
        {
            return await _context.UserSocialAccounts.FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId);
        }

        // 소셜 조회
        public async Task<UserSocialAccountEntity?> FindByUserIdAsnc(long id)
        {
            return await _context.UserSocialAccounts.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public void Add(UserSocialAccountEntity socialAccount)
        {
            _context.UserSocialAccounts.Add(socialAccount);
        }

        public void SaveChange()
        {
            _context.SaveChanges();
        }
    }
}
