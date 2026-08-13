using MovieReviewApi.External.TMDB;

namespace MovieReviewApi.DTOs.Tvs
{
    public class TvsPageResponse
    {
        public int CurrentPage  { get; set; }
        public int TotalPages   { get; set; }
        public int TotalResults { get; set; }

        public int StartPage    { get; set; }
        public int EndPage      { get; set; }

        public List<TMDBTv> Results { get; set; } = [];
    }
}
