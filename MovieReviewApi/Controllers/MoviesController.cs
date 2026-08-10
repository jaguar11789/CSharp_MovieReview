using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReviewApi.Data;
using MovieReviewApi.Services.Movies;

namespace MovieReviewApi.Controllers
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
    }
}
