using MovieReviewApi.DTOs.Movies;
using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.Services.Movies
{
    public interface ITMDBMoviesService
    {
        Task<TMDBMoviesResponse> GetPopularMoviesAsync();

        Task<MoviesPageResponse> GetMoviesAsync(int page = 1);

        Task<MoviesPageResponse> SearchMoviesAsync(string query, int page);

        Task<MoviesPageResponse> GetMoviesByGenreAsync(int genreId, int page = 1);

        Task<TMDBMovie> GetMovieDetailAsync(long movieId);
    }
}
