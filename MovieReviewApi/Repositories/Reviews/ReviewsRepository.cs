using Microsoft.EntityFrameworkCore;
using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public class ReviewsRepository(AppDbContext context) : IReviewsRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<ReviewResponse>> GetByMovieIdAsync(long movieId, string sort = "latest")
        {
            var query = _context.Reviews.Where(x => x.MovieId == movieId && x.StatusCode == 0)
                                         .Join(_context.Users, review => review.UserId, user => user.Id, (review, user) => new ReviewResponse
                                         {
                                             Id = review.Id,
                                             MovieId = review.MovieId,
                                             UserId = user.UserId,
                                             Rating = review.Rating,
                                             Content = review.Content,

                                             CreatedAt = review.CreatedAt
                                         });
            query = sort switch
            {
                "rating" => query.OrderByDescending(x => x.Rating)
                                 .ThenByDescending(x => x.CreatedAt),
                                 _ => query.OrderByDescending(x => x.CreatedAt)
            };
            return await query.ToListAsync();                           
        }
        
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
