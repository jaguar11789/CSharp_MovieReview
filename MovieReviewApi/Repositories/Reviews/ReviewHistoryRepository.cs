using MovieReviewApi.Data;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public class ReviewHistoryRepository(AppDbContext context) : IReviewHistoryRepository
    {
        private readonly AppDbContext _context = context;

        public void Add(ReviewHistoryEntity reviewHistory)
        {
            _context.ReviewsHistories.Add(reviewHistory);
        }
    }
}
