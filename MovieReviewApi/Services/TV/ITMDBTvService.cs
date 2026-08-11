using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.Services.TV
{
    public interface ITMDBTvService
    {
        Task<TMDBTvResponse> GetPopularTvAsync();

        Task<TMDBTv> GetTvDetailAsync(long tvId);
    }
}
