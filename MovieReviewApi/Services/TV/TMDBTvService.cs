using MovieReviewApi.DTOs.Tvs;
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

        public async Task<TvsPageResponse> GetTvsAsync(int page = 1)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = "https://api.themoviedb.org/3/discover/tv" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             "&language=ko-KR" +
                             "&sort_by=popularity.desc" +
                             $"&page={page}";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 전체 TV 프로그램 요청 실패 : {tmdbResponseJson}");
            }

            var tvs = JsonSerializer.Deserialize<TMDBTvResponse>(tmdbResponseJson);

            if (tvs == null)
            {
                throw new InvalidOperationException("전체 TV 프로그램 응답을 처리하지 못했습니다.");
            }

            const int pageButtonCount = 10;

            var startPage = ((tvs.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage = Math.Min(startPage + pageButtonCount - 1, tvs.TotalPages);

            return new TvsPageResponse
            {
                CurrentPage  = tvs.Page,
                TotalPages   = tvs.TotalPages,
                TotalResults = tvs.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = tvs.Results
            };
        }

        public async Task<TvsPageResponse> SearchTvsAsync(string query, int page = 1)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = "https://api.themoviedb.org/3/search/tv" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             $"&language=ko-KR" +
                             $"&query={Uri.EscapeDataString(query)}" +
                             $"&page={page}" +
                             "&include_adult=false";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB TV 프로그램 검색 요청 실패 : {tmdbResponseJson}");
            }

            var tvs = JsonSerializer.Deserialize<TMDBTvResponse>(tmdbResponseJson);

            if (tvs == null)
            {
                throw new InvalidOperationException("TMDB TV 프로그램 검색 응답을 처리하지 못했습니다.");
            }

            const int pageButtonCount = 10;

            var startPage = ((tvs.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage   = Math.Min(startPage + pageButtonCount - 1, tvs.TotalPages);

            return new TvsPageResponse
            {
                CurrentPage  = tvs.Page,
                TotalPages   = tvs.TotalPages,
                TotalResults = tvs.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = tvs.Results
            };
        }

        public async Task<TvsPageResponse> GetTvsByGenresAsync(int genreId, int page = 1)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = $"https://api.themoviedb.org/3/discover/tv" +
                             $"?api_key={tmdbApiKey}" +
                             $"&language=ko-KR" +
                             $"&with_genres={genreId}" +
                             $"&page={page}";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 장르별 TV 프로그램 요청 실패 : {tmdbResponse.StatusCode}");
            }
            
            var tvs = JsonSerializer.Deserialize<TMDBTvResponse>(tmdbResponseJson);

            if (tvs == null)
            {
                throw new InvalidOperationException("TMDB 장르별 TV 프로그램 응답 데이터가 없습니다.");
            }

            const int pageButtonCount = 10;

            var startPage = ((tvs.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage = Math.Min(startPage + pageButtonCount - 1, tvs.TotalPages);

            return new TvsPageResponse
            {
                CurrentPage  = tvs.Page,
                TotalPages   = tvs.TotalPages,
                TotalResults = tvs.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = tvs.Results
            };
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
