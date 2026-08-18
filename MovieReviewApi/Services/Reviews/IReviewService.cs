using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Models.Reviews;

namespace MovieReviewApi.Services.Reviews
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetReviewsAsync(long movieId, string sort = "latest");

        Task<ResultResponse> CreateReviewAsync(long userId, CreateReviewRequest createReviewRequest);
    }
}
