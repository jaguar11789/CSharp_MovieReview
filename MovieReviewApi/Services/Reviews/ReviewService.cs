using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Models.Reviews;
using MovieReviewApi.Repositories.Reviews;

namespace MovieReviewApi.Services.Reviews
{
    public class ReviewService(AppDbContext context, IReviewsRepository reviewsRepository, IReviewHistoryRepository reviewHistoryRepository) : IReviewService
    {
        private readonly AppDbContext             _context                 = context;
        private readonly IReviewsRepository       _reviewRepository        = reviewsRepository;
        private readonly IReviewHistoryRepository _reviewHistoryRepository = reviewHistoryRepository;


        public async Task<ResultResponse> CreateReviewAsync(long userId, CreateReviewRequest createReviewRequest)
        {
            if (createReviewRequest.Rating < 1 || createReviewRequest.Rating > 5)
            {
                return new ResultResponse
                {
                    retVal = 900,
                    retMsg = "별점은 1점에서 5점 사이여야 합니다."
                };
            }

            if (string.IsNullOrWhiteSpace(createReviewRequest.Content))
            {
                return new ResultResponse
                {
                    retVal = 910,
                    retMsg = "리뷰 내용을 입력해주세요."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var review = new ReviewsEntity
                {
                    UserId     = userId,
                    MovieId    = createReviewRequest.MovieId,
                    Rating     = createReviewRequest.Rating,
                    Content    = createReviewRequest.Content,
                    StatusCode = 0,

                    CreatedAt  = DateTime.Now
                };

                var history = new ReviewHistoryEntity
                {
                    UserId     = userId,
                    ActionCode = 100,
                    Rating     = createReviewRequest.Rating,
                    Content    = createReviewRequest.Content,
                    StatusCode = 0,

                    Memo       = "리뷰 작성",
                    CreatedAt  = DateTime.Now,
                    Reviews    = review
                };

                _reviewRepository.Add(review);
                _reviewHistoryRepository.Add(history);

                await _reviewRepository.SaveChangeAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "리뷰가 등록 되었습니다."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine(ex.ToString());

                return new ResultResponse
                {
                    retVal = 999,
                    retMsg = "리뷰 등록 중 오류가 발생했습니다."
                };
            }
        }
    }
}
