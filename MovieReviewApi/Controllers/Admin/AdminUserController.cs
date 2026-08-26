using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Services.Admin;

namespace MovieReviewApi.Controllers.Admin
{
    [Route("api/[controller]/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController(IAdminUserService adminUserService) : ControllerBase
    {
        private readonly IAdminUserService _adminUserService = adminUserService;

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int     page       = 1,
                                                  [FromQuery] int     pageSize   = 10,
                                                  [FromQuery] string? keyword    = null,
                                                  [FromQuery] int?    statusCode = null,
                                                  [FromQuery] int?    joinDays   = null)
        {
            var result = await _adminUserService.GetAllAsync(page, pageSize, keyword, statusCode, joinDays);

            return Ok(result);
        }
    }
}