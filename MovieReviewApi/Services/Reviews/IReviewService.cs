using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Reviews;

namespace MovieReviewApi.Services.Reviews
{
    public interface IReviewService
    {
        Task<ResultResponse> CreateReviewAsync(long userId, CreateReviewRequest createReviewRequest);
    }
}
