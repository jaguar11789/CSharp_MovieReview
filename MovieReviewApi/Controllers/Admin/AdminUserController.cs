using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Common.Responses;
using MovieReviewApi.DTOs.Admin;
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

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var result = await _adminUserService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new ResultResponse
                {
                    retVal = 404,
                    retMsg = "회원을 찾을 수 없습니다."
                });
            }
            
            return Ok(result);
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> UpdateStatus(long id, AdminUserStatusUpdateRequest adminUserStatusUpdateRequest)
        {
            var result = await _adminUserService.UpdateStatusAsync(id, adminUserStatusUpdateRequest.StatusCode, adminUserStatusUpdateRequest.Memo);

            return Ok(result);
        }
    }
}