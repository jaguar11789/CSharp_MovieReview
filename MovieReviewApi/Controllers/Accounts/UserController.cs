using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.DTOs.Accounts.User;
using MovieReviewApi.Services.Accounts.User;
using System.Security.Claims;

namespace MovieReviewApi.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("mypage")]
        public async Task<IActionResult> MyPage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            var result = await _userService.GetMyPageAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest passwordChangeRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse(userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _userService.ChangePasswordAsync(id, passwordChangeRequest);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("user")]
        public async Task<IActionResult> ChangeUserInfo(UserUpdateRequest userUpdateRequest)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!long.TryParse (userId, out var id))
            {
                return Unauthorized();
            }
            var result = await _userService.ChangeUserInfoAsync(id, userUpdateRequest);

            return Ok(result);
        }
    }
}
