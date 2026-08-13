using MovieReviewApi.Data;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public class ReviewsRepository(AppDbContext context) : IReviewsRepository
    {
        private readonly AppDbContext _context = context;

        public void Add(ReviewsEntity review)
        {
            _context.Reviews.Add(review);
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
