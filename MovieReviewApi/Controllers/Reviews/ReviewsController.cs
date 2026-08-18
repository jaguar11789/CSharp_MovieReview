using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.DTOs.Reviews;
using MovieReviewApi.Services.Accounts.User;
using MovieReviewApi.Services.Reviews;
using System.Security.Claims;

namespace MovieReviewApi.Controllers.Reviews
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController(IUserService userService, IReviewService reviewService) : ControllerBase
    {
        private readonly IUserService   _userService   = userService;
        private readonly IReviewService _reviewService = reviewService;

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetReviews(long movieId, string sort = "latest")
        {
            var reviews = await _reviewService.GetReviewsAsync(movieId, sort);

            return Ok(reviews);
        }

        // 리뷰 등록
        [Authorize]
        [HttpPost("review")]
        public async Task<IActionResult> CreateReview(CreateReviewRequest createReviewRequest)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var result = await _reviewService.CreateReviewAsync(userId, createReviewRequest);

            return Ok(result);
        }

        // 리뷰 수정
        [Authorize]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(long reviewId, ReviewRequest reviewRequest)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var result = await _reviewService.UpdateReviewAsync(userId, reviewId, reviewRequest);

            return Ok(result);
        }

        // 리뷰 삭제
        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(long reviewId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var result = await _reviewService.DeleteReviewAsync(userId, reviewId);

            return Ok(result);
        }
    }
}
