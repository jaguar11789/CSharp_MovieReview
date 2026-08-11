using MovieReviewApi.External.TMDB;
using System.Text.Json;

namespace MovieReviewApi.Services.TV
{
    public class TMDBTvService(HttpClient httpClient, IConfiguration configuration) : ITMDBTvService
    {
        private readonly HttpClient     _httpClient    = httpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<TMDBTvResponse> GetPopularTvAsync()
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = "https://api.themoviedb.org/3/tv/popular" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             "&language=ko-KR" +
                             $"&page=1";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 인기 TV 프로그램 요청 실패 : {tmdbResponseJson}");
            }

            var tvData = JsonSerializer.Deserialize<TMDBTvResponse>(tmdbResponseJson);

            if (tvData == null)
            {
                throw new InvalidOperationException("TMDB TV 프로그램 응답을 처리하지 못했습니다.");
            }

            return tvData;
        }

        public async Task<TMDBTv> GetTvDetailAsync(long tvId)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = $"https://api.themoviedb.org/3/tv/{tvId}" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             "&language=ko-KR" +
                             "&append_to_response=credits";

            var tmdbResponse = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB TV 프로그램 상세 정보 요청 실패 : {tmdbResponse}");

            }

            var tv = JsonSerializer.Deserialize<TMDBTv>(tmdbResponseJson);

            if (tv == null)
            {
                throw new InvalidOperationException("TMDB TV 프로그램 상세 정보 응답을 처리하지 못했습니다.");
            }
            return tv;
        }
    }
}
