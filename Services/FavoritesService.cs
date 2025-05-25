using Microsoft.JSInterop;
using NowPlaying.Models;
using System.Text.Json;

namespace NowPlaying.Services
{
    public class FavoritesService(IJSRuntime jSRuntime)
    {
        private readonly string _localStorageKey = "favorites";

        /// <summary>
        /// Returns a list of favorite movies from local storage.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Movie>> GetFavorites()
        {
            List<Movie> movies = [];

            try
            {
                var json = await jSRuntime.InvokeAsync<string>("localStorage.getItem", _localStorageKey);
                movies = JsonSerializer.Deserialize<List<Movie>>(json) ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving favorites: {ex.Message}");
            }

            return movies;
        }

        /// <summary>
        /// Saves a list of movies to local storage.
        /// </summary>
        /// <param name="movies">a list of movies</param>
        /// <returns></returns>
        public async Task SaveFavorites(List<Movie> movies)
        {
            try
            {
                var json = JsonSerializer.Serialize(movies);
                await jSRuntime.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving favorites: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a movie to the favorites list if it is not already present.
        /// </summary>
        /// <param name="movie">a movie item</param>
        /// <returns></returns>
        public async Task AddFavorite(Movie movie)
        {
            var currentMovies = await GetFavorites();

            if (currentMovies.All(m => m.Id != movie.Id))
            {
                currentMovies.Add(movie);
                await SaveFavorites(currentMovies);
            }
        }

        /// <summary>
        /// Removes a movie from the favorites list in storage.
        /// </summary>
        /// <param name="movie">a movie item</param>
        /// <returns></returns>
        public async Task RemoveFavorite(Movie movie)
        {
            var currentMovies = await GetFavorites();
            currentMovies = currentMovies.Where(f => f.Id != movie.Id).ToList();

            await SaveFavorites(currentMovies);
        }

        /// <summary>
        /// checks to see if movie is in list of favorites
        /// </summary>
        /// <param name="id">Movie Id</param>
        /// <returns></returns>
        public async Task<bool> IsFavorite(int id)
        {
            var currentMovies = await GetFavorites();

            bool IsFavorite = currentMovies.Any(f => f.Id == id);

            return IsFavorite;
        }

    }
}
