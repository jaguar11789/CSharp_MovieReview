using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.Services.Movies
{
    public interface ITMDBMoviesService
    {
        Task<TMDBMoviesResponse> GetPopularMoviesAsync();

        Task<TMDBMoviesResponse> GetMoviesAsync(int page = 1);

        Task<TMDBMoviesResponse> SearchMoviesAsync(string query, int page);

        Task<TMDBMovie> GetMovieDetailAsync(long movieId);
    }
}
