using Azure;
using MovieReviewApi.DTOs.Movies;
using MovieReviewApi.External.TMDB;
using System.Text.Json;

namespace MovieReviewApi.Services.Movies
{
    public class TMDBMoviesService(HttpClient httpClient, IConfiguration configuration) : ITMDBMoviesService
    {
        private readonly HttpClient     _httpClient    = httpClient;
        private readonly IConfiguration _configuration = configuration;

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

        public async Task<MoviesPageResponse> GetMoviesAsync(int page = 1)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = "https://api.themoviedb.org/3/discover/movie" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             "&language=ko-KR" +
                             "&sort_by=popularity.desc" +
                             $"&page={page}";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 전체 영화 요청 실패 : {tmdbResponseJson}");
            }

            var movies = JsonSerializer.Deserialize<TMDBMoviesResponse>(tmdbResponseJson);

            if (movies == null)
            {
                throw new InvalidOperationException("전체 영화 응답을 처리하지 못했습니다.");
            }

            const int pageButtonCount = 10;

            var startPage = ((movies.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage   = Math.Min(startPage + pageButtonCount - 1, movies.TotalPages);

            return new MoviesPageResponse
            {
                CurrentPage  = movies.Page,
                TotalPages   = movies.TotalPages,
                TotalResults = movies.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = movies.Results
            };
        }

        public async Task<MoviesPageResponse> SearchMoviesAsync(string query, int page)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = "https://api.themoviedb.org/3/search/movie" +
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             $"&language=ko-KR" +
                             $"&query={Uri.EscapeDataString(query)}" +
                             $"&page={page}" +
                             "&include_adult=false";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 영화 검색 요청 실패 : {tmdbResponseJson}");
            }

            var movies = JsonSerializer.Deserialize<TMDBMoviesResponse>(tmdbResponseJson);

            if (movies == null)
            {
                throw new InvalidOperationException("TMDB 영화 검색 응답을 처리하지 못했습니다.");
            }

            const int pageButtonCount = 10;

            var startPage = ((movies.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage   = Math.Min(startPage + pageButtonCount - 1, movies.TotalPages);

            return new MoviesPageResponse
            {
                CurrentPage  = movies.Page,
                TotalPages   = movies.TotalPages,
                TotalResults = movies.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = movies.Results
            };
        }

        public async Task<MoviesPageResponse> GetMoviesByGenreAsync(int genreId, int page = 1)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = $"https://api.themoviedb.org/3/discover/movie" +
                             $"?api_key={tmdbApiKey}" +
                             $"&language=ko-KR" +
                             $"&with_genres={genreId}" +
                             $"&page={page}";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 장르별 영화 요청 실패 : {tmdbResponse.StatusCode}");
            }

            var movies = JsonSerializer.Deserialize<TMDBMoviesResponse>(tmdbResponseJson);

            if (movies == null)
            {
                throw new InvalidOperationException("TMDB 장르별 영화 응답 데이터가 없습니다.");
            }
            const int pageButtonCount = 10;

            var startPage = ((movies.Page - 1) / pageButtonCount) * pageButtonCount + 1;
            var endPage   = Math.Min(startPage + pageButtonCount - 1, movies.TotalPages);

            return new MoviesPageResponse
            {
                CurrentPage  = movies.Page,
                TotalPages   = movies.TotalPages,
                TotalResults = movies.TotalResults,
                StartPage    = startPage,
                EndPage      = endPage,

                Results      = movies.Results
            };
        }

        public async Task<TMDBMovie> GetMovieDetailAsync(long movieId)
        {
            var tmdbApiKey = _configuration["TMDB:ApiKey"];

            if (string.IsNullOrEmpty(tmdbApiKey))
            {
                throw new InvalidOperationException("TMDB API KEY가 설정되지 않았습니다.");
            }

            var tmdbApiUrl = $"https://api.themoviedb.org/3/movie/{movieId}" + 
                             $"?api_key={Uri.EscapeDataString(tmdbApiKey)}" +
                             "&language=ko-KR" +
                             "&append_to_response=credits";

            var tmdbResponse     = await _httpClient.GetAsync(tmdbApiUrl);
            var tmdbResponseJson = await tmdbResponse.Content.ReadAsStringAsync();

            if (!tmdbResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"TMDB 영화 상세 정보 요청 실패 : {tmdbResponse}");
            }

            var movie = JsonSerializer.Deserialize<TMDBMovie>(tmdbResponseJson);

            if (movie == null)
            {
                throw new InvalidOperationException("TMDB 영화 상세 정보 응답을 처리하지 못했습니다.");
            }
            return movie;
        }
    }
}
