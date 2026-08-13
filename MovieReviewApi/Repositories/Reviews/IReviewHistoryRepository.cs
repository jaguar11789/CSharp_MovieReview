using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Repositories.Reviews
{
    public interface IReviewHistoryRepository
    {
        void Add(ReviewHistoryEntity reviewHistory);
    }
}
