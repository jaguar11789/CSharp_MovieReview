using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public interface IReviewsRepository
    {
        Task<List<ReviewResponse>> GetByMovieIdAsync(long movieId, string sort = "latest");

        Task<ReviewsEntity?> GetByIdAsync(long reviewId);

        void Add(ReviewsEntity reviewsEntity);

        Task SaveChangeAsync();
    }
}
