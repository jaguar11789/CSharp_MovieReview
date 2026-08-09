using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public interface IUserSocialAccountRepository
    {
        Task <UserSocialAccountEntity?> FindByProviderUserIdAsync(string provider, string providerUserId);

        Task<UserSocialAccountEntity?> FindByUserIdAsnc(long id);

        void Add(UserSocialAccountEntity socialAccount);

        void SaveChange();
    }
}
