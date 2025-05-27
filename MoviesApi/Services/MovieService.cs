using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Helpers;
using MoviesApi.Models;
using MoviesApi.Services.Interfaces;

namespace MoviesApi.Services
{
    public class MovieService : IMovieService
    {
        private readonly MoviesDbContext _context;

        public MovieService(MoviesDbContext context)
        {
            _context = context;
        }

        public IEnumerable<object> GetFilteredMovies(string? title, int? year, string? genre)
        {
            var query = _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Ratings)
                .AsQueryable();

            if (title != null) query = query.Where(m => m.Title.Contains(title));
            if (year.HasValue) query = query.Where(m => m.YearOfRelease == year);
            if (genre != null) query = query.Where(m => m.Genres.Any(g => g.Name == genre));

            return query
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.YearOfRelease,
                    m.RunningTime,
                    Genres = m.Genres.Select(g => g.Name),
                    AverageRating = RatingHelper.RoundToNearest(m.Ratings.Any() ? m.Ratings.Average(r => r.Value) : 0, 0.5)
                })
                .ToList();
        }

        public IEnumerable<object> GetTopRated()
        {
            return _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Ratings)
                .Select(m => new
                {
                    Movie = m,
                    Avg = m.Ratings.Any() ? m.Ratings.Average(r => r.Value) : 0
                })
                .OrderByDescending(x => x.Avg)
                .ThenBy(x => x.Movie.Title)
                .Take(5)
                .Select(x => new
                {
                    x.Movie.Id,
                    x.Movie.Title,
                    x.Movie.YearOfRelease,
                    x.Movie.RunningTime,
                    Genres = x.Movie.Genres.Select(g => g.Name),
                    AverageRating = RatingHelper.RoundToNearest(x.Avg, 0.5)
                })
                .ToList();
        }

        public IEnumerable<object> GetTopRatedByUser(int userId)
        {
            return _context.Ratings
                .Where(r => r.UserId == userId)
                .Include(r => r.Movie).ThenInclude(m => m.Genres)
                .ToList()
                .GroupBy(r => r.Movie)
                .Select(g => new
                {
                    Movie = g.Key,
                    Score = g.Max(r => r.Value)
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Movie.Title)
                .Take(5)
                .Select(x => new
                {
                    x.Movie.Id,
                    x.Movie.Title,
                    x.Movie.YearOfRelease,
                    x.Movie.RunningTime,
                    Genres = x.Movie.Genres.Select(g => g.Name),
                    AverageRating = RatingHelper.RoundToNearest(x.Movie.Ratings.Average(r => r.Value), 0.5)
                })
                .ToList();
        }

        public void RateMovie(int userId, int movieId, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var user = _context.Users.Find(userId);
            var movie = _context.Movies.Find(movieId);

            if (user == null || movie == null)
                throw new InvalidOperationException("User or Movie not found.");

            var existing = _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.MovieId == movieId);
            if (existing != null)
                existing.Value = rating;
            else
                _context.Ratings.Add(new Rating { User = user, Movie = movie, Value = rating });

            _context.SaveChanges();
        }
    }
}
