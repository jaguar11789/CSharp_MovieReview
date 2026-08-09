using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public interface IUserHistoryRepository
    {
        void Add(UserHistoryEntity userHistory);
    }
}
