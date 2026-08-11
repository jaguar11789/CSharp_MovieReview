using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.DTOs.Movies
{
    public class MoviesPageResponse
    {
        public int CurrentPage  { get; set; }
        public int TotalPages   { get; set; }
        public int TotalResults { get; set; }

        public int StartPage    { get; set; }
        public int EndPage      { get; set; }

        public List<TMDBMovie> Results { get; set; } = new();
    }
}
