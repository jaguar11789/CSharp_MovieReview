using System.Text.Json.Serialization;

namespace MovieReviewApi.External.TMDB
{
    public class TMDBMoviesResponse
    {
        [JsonPropertyName("page")]
        public int             Page         { get; set; }

        [JsonPropertyName("results")]
        public List<TMDBMovie> Results      { get; set; } = new();

        [JsonPropertyName("total_pages")]
        public int             TotalPages   { get; set; }

        [JsonPropertyName("total_results")]
        public int             TotalResults { get; set; }
    }

    public class TMDBMovie
    {
        [JsonPropertyName("id")]
        public int     Id           { get; set; }

        [JsonPropertyName("title")]
        public string? Title        { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview     { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath   { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate  { get; set; }

        [JsonPropertyName("vote_average")]
        public double  VoteAverage  { get; set; }

        [JsonPropertyName("vote_count")]
        public int     VoteCount    { get; set; }

        [JsonPropertyName("popularity")]
        public double  Popularity   { get; set; }
    }
}
