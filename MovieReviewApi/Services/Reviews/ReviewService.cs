using Azure.Core;
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

        public async Task<List<ReviewResponse>> GetReviewsAsync(long movieId, string sort = "latest")
        {
            return await _reviewRepository.GetByMovieIdAsync(movieId, sort);
        }

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

        public async Task<ResultResponse> UpdateReviewAsync(long userId, long reviewId, ReviewRequest reviewRequest)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null)
            {
                return new ResultResponse
                {
                    retVal = 404,
                    retMsg = "리뷰를 찾을 수 없습니다."
                };
            }

            if (review.UserId != userId)
            {
                return new ResultResponse
                {
                    retVal = 403,
                    retMsg = "본인이 작성한 리뷰만 수정할 수 있습니다."
                };
            }

            if (review.StatusCode != 0)
            {
                return new ResultResponse
                {
                    retVal = 410,
                    retMsg = "이미 삭제된 리뷰입니다."
                };
            }

            if (reviewRequest.Rating < 1 || reviewRequest.Rating > 5)
            {
                return new ResultResponse
                {
                    retVal = 900,
                    retMsg = "별점은 1점에서 5점 사이여야 합니다."
                };
            }

            if (string.IsNullOrWhiteSpace(reviewRequest.Content))
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
                review.Rating    = reviewRequest.Rating;
                review.Content   = reviewRequest.Content;
                review.UpdatedAt = DateTime.Now;

                var history = new ReviewHistoryEntity
                {
                    Reviews    = review,
                    UserId     = userId,
                    ActionCode = 200,
                    Rating     = review.Rating,
                    Content    = review.Content,

                    StatusCode = review.StatusCode,
                    Memo       = "리뷰 수정",
                    CreatedAt  = DateTime.Now
                };

                _reviewHistoryRepository.Add(history);

                await _reviewRepository.SaveChangeAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "리뷰가 수정되었습니다."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine(ex);

                return new ResultResponse
                {
                    retVal = 999,
                    retMsg = "리뷰 수정 중 오류가 발생했습니다."
                };
            }
        }

        public async Task<ResultResponse> DeleteReviewAsync(long userId, long reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);

            if (review == null)
            {
                return new ResultResponse
                {
                    retVal = 404,
                    retMsg = "리뷰를 찾을 수 없습니다."
                };
            }

            if (review.UserId != userId)
            {
                return new ResultResponse
                {
                    retVal = 403,
                    retMsg = "본인이 작성한 리뷰만 삭제할 수 있습니다."
                };
            }

            if (review.StatusCode != 0)
            {
                return new ResultResponse
                {
                    retVal = 410,
                    retMsg = "이미 삭제된 리뷰입니다."
                };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                review.StatusCode = 999;
                review.UpdatedAt  = DateTime.Now;

                var history = new ReviewHistoryEntity
                {
                    Reviews    = review,
                    UserId     = userId,
                    ActionCode = 900,
                    Rating     = review.Rating,
                    Content    = review.Content,

                    StatusCode = 999,
                    Memo       = "리뷰 삭제",
                    CreatedAt  = DateTime.Now
                };

                _reviewHistoryRepository.Add(history);

                await _reviewRepository.SaveChangeAsync();
                await transaction.CommitAsync();

                return new ResultResponse
                {
                    retVal = 0,
                    retMsg = "리뷰가 삭제되었습니다."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine(ex);

                return new ResultResponse
                {
                    retVal = 999,
                    retMsg = "리뷰 삭제 중 오류가 발생했습니다."
                };
            }
        }
    }
}
