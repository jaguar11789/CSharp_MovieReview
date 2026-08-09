using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.Services.Movies
{
    public interface ITMDBMoviesService
    {
        Task<TMDBMoviesResponse> GetPopularMoviesAsync();
    }
}
