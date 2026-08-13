using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Common.Responses;
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

        [HttpGet]
        public async Task<IActionResult> GetTvs([FromQuery] int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            
            var tvs = await _tmdbTvService.GetTvsAsync(page);

            return Ok(tvs);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTvs([FromQuery] string query, [FromQuery] int page = 1)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest(new ResultResponse
                {
                    retVal = 400,
                    retMsg = "검색어를 입력해주세요."
                });
            }

            if (page < 1)
            {
                page = 1;
            }

            var tvs = await _tmdbTvService.SearchTvsAsync(query, page);

            return Ok(tvs);
        }

        [HttpGet("genre/{genreId}")]
        public async Task<IActionResult> GetTvsByGenre(int genreId, [FromQuery] int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            var tvs = await _tmdbTvService.GetTvsByGenresAsync(genreId, page);

            return Ok(tvs);
        }

        [HttpGet("{tvId:long}")]
        public async Task<IActionResult> GetTvDetail(long tvId)
        {
            var tv = await _tmdbTvService.GetTvDetailAsync(tvId);

            return Ok(tv);
        }
    }
}
