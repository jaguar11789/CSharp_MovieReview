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
        public int               Id           { get; set; }

        [JsonPropertyName("title")]
        public string?           Title        { get; set; }

        [JsonPropertyName("overview")]
        public string?           Overview     { get; set; }

        [JsonPropertyName("poster_path")]
        public string?           PosterPath   { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string?           BackdropPath { get; set; }

        [JsonPropertyName("release_date")]
        public string?           ReleaseDate  { get; set; }

        [JsonPropertyName("vote_average")]
        public double            VoteAverage  { get; set; }

        [JsonPropertyName("vote_count")]
        public int               VoteCount    { get; set; }

        [JsonPropertyName("popularity")]
        public double            Popularity   { get; set; }

        [JsonPropertyName("runtime")]
        public int?              Runtime      { get; set; }

        [JsonPropertyName("genres")]
        public List<TMDBGenre>   Genres       { get; set; } = [];

        [JsonPropertyName("credits")]
        public TMDBMovieCredits? Credits      { get; set; }
    }

    public class TMDBGenre
    {
        [JsonPropertyName("id")]
        public int     Id   { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class TMDBMovieCredits 
    { 
        [JsonPropertyName("cast")]
        public List<TMDBCast> Cast { get; set; } = [];

        [JsonPropertyName("crew")]
        public List<TMDBCrew> Crew { get; set; } = [];
    }

    public class TMDBCast 
    {
        [JsonPropertyName("id")]
        public int     Id          { get; set; }

        [JsonPropertyName("name")]
        public string? Name        { get; set; }

        [JsonPropertyName("character")]
        public string? Character   { get; set; }

        [JsonPropertyName("profile_path")]
        public string? ProfilePath { get; set; }

        [JsonPropertyName("order")]
        public int?    Order       { get; set; }
    }

    public class TMDBCrew
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("job")]
        public string? Job { get; set; }

        [JsonPropertyName("department")]
        public string? Department { get; set; }
    }
}
