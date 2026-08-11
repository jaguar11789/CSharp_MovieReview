using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Services.TV;

namespace MovieReviewApi.Controllers.TMDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class TvsController(ITMDBTvService tmdbTvService) : ControllerBase
    {
        private readonly ITMDBTvService _tmdbTvService = tmdbTvService;

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularTvs()
        {
            var tv = await _tmdbTvService.GetPopularTvAsync();

            return Ok(tv);
        }

        [HttpGet("{tvId:long}")]
        public async Task<IActionResult> GetTvDetail(long tvId)
        {
            var tv = await _tmdbTvService.GetTvDetailAsync(tvId);

            return Ok(tv);
        }
    }
}
