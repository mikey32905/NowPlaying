using NowPlaying.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NowPlaying.Services
{
    public class TMDBService
    {
        private readonly HttpClient _http;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public TMDBService(HttpClient http, IConfiguration config)
        {
            _http = http;

            string? tmdbKey = config["TmdbAccessKey"];

            if (!string.IsNullOrWhiteSpace(tmdbKey))
            {
                _http.DefaultRequestHeaders.Authorization = new("Bearer", tmdbKey);
            }
        }

        public async Task<MovieListResponse> GetNowPlayingMovies()
        {
            string url = "https://api.themoviedb.org/3/movie/now_playing?region=US&language=en-US";
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";

            MovieListResponse response = await _http.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Now Showing Movies could not be loaded.");

            foreach (var movie in response.Results)
            {
                if (string.IsNullOrEmpty(movie.PosterPath))
                {
                    movie.PosterPath = "/img/mw1920_poster.png";
                }
                else
                {
                    movie.PosterPath = $"{imageBaseUrl}{movie.PosterPath}";
                }
            }

            return response;
        }

        public async Task<MovieListResponse> GetPopularMovies()
        {
            string url = "https://api.themoviedb.org/3/movie/popular?region=US&language=en-US";
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";

            MovieListResponse response = await _http.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
            ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Popular Movies could not be loaded.");

            foreach (var movie in response.Results)
            {
                if (string.IsNullOrEmpty(movie.PosterPath))
                {
                    movie.PosterPath = "/img/mw1920_poster.png";
                }
                else
                {
                    movie.PosterPath = $"{imageBaseUrl}{movie.PosterPath}";
                }
            }

            return response;

        }

        /// <summary>
        /// Search for movies
        /// </summary>
        /// <param name="query">User supplied query</param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<MovieListResponse> SearchMovies(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?query={query}&include_adult=false&language=en-US";
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";

            MovieListResponse response = await _http.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
            ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Search results could not be loaded.");

            foreach (var movie in response.Results)
            {
                if (string.IsNullOrEmpty(movie.PosterPath))
                {
                    movie.PosterPath = "/img/mw1920_poster.png";
                }
                else
                {
                    movie.PosterPath = $"{imageBaseUrl}{movie.PosterPath}";
                }
            }

            return response;

        }

        /// <summary>
        /// Get movie details by ID
        /// </summary>
        /// <param name="movieId">id of movie</param>
        /// <returns></returns>
        /// <exception cref="HttpIOException"></exception>
        public async Task<MovieDetails> GetMovieById(int movieId)
        {
            string url = $"https://api.themoviedb.org/3/movie/{movieId}";
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";

            MovieDetails? response = await _http.GetFromJsonAsync<MovieDetails>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Could not retrieve movie details!");

            if (string.IsNullOrEmpty(response.PosterPath))
            {
                response.PosterPath = "/img/mw1920_poster.png";
            }
            else
            {
                response.PosterPath = $"{imageBaseUrl}{response.PosterPath}";
            }

            if (string.IsNullOrEmpty(response.BackdropPath))
            {
                response.BackdropPath = "/img/mw1920_backdrop.png";
            }
            else
            {
                response.BackdropPath = $"{imageBaseUrl}{response.BackdropPath}";
            }

            return response;
        }

        /// <summary>
        /// retrieves a movie trailer by movie ID
        /// </summary>
        /// <param name="movieId">id of movie</param>
        /// <returns></returns>
        public async Task<Video?> GetMovieTrailer(int movieId)
        {
            string url = $"https://api.themoviedb.org/3/movie/{movieId}/videos?language=en-US";

            var videos = await _http.GetFromJsonAsync<MovieVideosResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Could not retrieve movie trailer!");

            return videos.Results.FirstOrDefault(v => v.Site!.Contains("YouTube", StringComparison.OrdinalIgnoreCase)
                                                   && v.Type!.Equals("Trailer", StringComparison.OrdinalIgnoreCase));

            //return movieTrailer;
        }

    }
}
