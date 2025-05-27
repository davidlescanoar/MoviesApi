using MoviesApi.Models;

namespace MoviesApi.Data
{
    public static class DbSeeder
    {
        public static void Seed(MoviesDbContext context)
        {
            if (context.Movies.Any()) return;

            var action = new Genre { Name = "Action" };
            var drama = new Genre { Name = "Drama" };
            var scifi = new Genre { Name = "Sci-Fi" };
            var crime = new Genre { Name = "Crime" };

            var inception = new Movie
            {
                Title = "Inception",
                YearOfRelease = 2010,
                RunningTime = 148,
                Genres = new List<Genre> { action, scifi }
            };

            var godfather = new Movie
            {
                Title = "The Godfather",
                YearOfRelease = 1972,
                RunningTime = 175,
                Genres = new List<Genre> { drama, crime }
            };

            var matrix = new Movie
            {
                Title = "The Matrix",
                YearOfRelease = 1999,
                RunningTime = 136,
                Genres = new List<Genre> { action, scifi }
            };

            var alice = new User { Name = "Alice" };
            var bob = new User { Name = "Bob" };

            var ratings = new List<Rating>
            {
                new() { Movie = inception, User = alice, Value = 5 },
                new() { Movie = inception, User = bob, Value = 4 },
                new() { Movie = godfather, User = alice, Value = 5 },
                new() { Movie = godfather, User = bob, Value = 5 },
                new() { Movie = matrix, User = alice, Value = 4 }
            };

            context.AddRange(inception, godfather, matrix);
            context.AddRange(alice, bob);
            context.AddRange(ratings);

            context.SaveChanges();
        }
    }
}
