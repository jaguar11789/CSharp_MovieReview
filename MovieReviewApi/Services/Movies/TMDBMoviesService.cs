using MovieReviewApi.External.TMDB;
using System.Text.Json;

namespace MovieReviewApi.Services.Movies
{
    public class TMDBMoviesService : ITMDBMoviesService
    {
        private readonly HttpClient     _httpClient;
        private readonly IConfiguration _configuration;

        public TMDBMoviesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient    = httpClient;
            _configuration = configuration;
        }

        public async Task<TMDBMoviesResponse> GetPopularMoviesAsync()
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }
            var tmdbApiUrl = "https://api.themoviedb.org/3/movie/popular" +
                      $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                      "&language=ko-KR" +
                      "&page=1";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 인기 영화 요청 실패 : {tmdbResponseJson}");
            }
            
            var popularMovies = JsonSerializer.Deserialize<TMDBMoviesResponse>(tmdbResponseJson);

            if (popularMovies == null)
            {
                throw new InvalidOperationException("TMDB 인기 영화 응답을 처리하지 못했습니다.");
            }
            return popularMovies;
        }
    }
}
