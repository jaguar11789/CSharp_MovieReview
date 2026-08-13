using MovieReviewApi.DTOs.Tvs;
using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.Services.TV
{
    public interface ITMDBTvService
    {
        Task<TMDBTvResponse> GetPopularTvAsync();

        Task<TvsPageResponse> GetTvsAsync(int page = 1);

        Task<TvsPageResponse> SearchTvsAsync(string query, int page = 1);

        Task<TvsPageResponse> GetTvsByGenresAsync(int genreId, int page = 1);

        Task<TMDBTv> GetTvDetailAsync(long tvId);
    }
}
