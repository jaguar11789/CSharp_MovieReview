using MovieReviewApi.Data;
using MovieReviewApi.Models.Accounts.User;

namespace MovieReviewApi.Repositories.User
{
    public class UserHistoryRepository : IUserHistoryRepository
    {
        private readonly AppDbContext _context;
        
        public UserHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(UserHistoryEntity userHistory)
        {
            _context.UserHistories.Add(userHistory);
        }
    }
}
