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

    }
}
