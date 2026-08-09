using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> ExistsByUserIdAsync(string userId);

        Task<UserEntity?> FindByUserIdAsync(string userId);

        Task<UserEntity?> FindByIdAsync(long id);

        void Add(UserEntity userEntity);

        // Task UpdateAsync(UserEntity userEntity);

        Task SaveChangeAsync();
    }
}
