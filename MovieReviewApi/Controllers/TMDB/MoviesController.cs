using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Common.Responses;
using MovieReviewApi.Data;
using MovieReviewApi.Services.Movies;

namespace MovieReviewApi.Controllers.TMDB
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController(ITMDBMoviesService tmdbMoviesService) : ControllerBase
    {
        private readonly ITMDBMoviesService _tmdbMoviesService = tmdbMoviesService;

        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularMovies()
        {
            var popularMovies = await _tmdbMoviesService.GetPopularMoviesAsync();

            return Ok(popularMovies);
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            var movies = await _tmdbMoviesService.GetMoviesAsync(page);

            return Ok(movies);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies([FromQuery] string query, [FromQuery] int page = 1)
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

            var movies = await _tmdbMoviesService.SearchMoviesAsync(query, page);

            return Ok(movies);
        }

        [HttpGet("genre/{genreId}")]
        public async Task<IActionResult> GetMoivesByGenre(int genreId, [FromQuery] int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }

            var movies = await _tmdbMoviesService.GetMoviesByGenreAsync(genreId, page);

            return Ok(movies);
        }

        [HttpGet("{movieId:long}")]
        public async Task<IActionResult> GetMovieDetail(long movieId)
        {
            var movie = await _tmdbMoviesService.GetMovieDetailAsync(movieId);

            return Ok(movie);
        }
    }
}
