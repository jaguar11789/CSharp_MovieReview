using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public interface IReviewsRepository
    {
        Task<List<ReviewResponse>> GetByMovieIdAsync(long movieId, string sort = "latest");

        void Add(ReviewsEntity reviewsEntity);

        Task SaveChangeAsync();
    }
}
