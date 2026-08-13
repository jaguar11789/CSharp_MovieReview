using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public interface IReviewsRepository
    {
        void Add(ReviewsEntity reviewsEntity);

        Task SaveChangeAsync();
    }
}
