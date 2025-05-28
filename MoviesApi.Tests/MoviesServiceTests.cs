using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Tests;

public class MovieServiceTests
{
    private MoviesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MoviesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new MoviesDbContext(options);

        var action = new Genre { Name = "Action" };
        var drama = new Genre { Name = "Drama" };

        var movie = new Movie
        {
            Title = "The Matrix",
            YearOfRelease = 1999,
            RunningTime = 130,
            Genres = new List<Genre> { action }
        };

        var movie2 = new Movie
        {
            Title = "Drama Movie",
            YearOfRelease = 2005,
            RunningTime = 90,
            Genres = new List<Genre> { drama }
        };

        var user = new User { Name = "Test User" };

        context.AddRange(movie, movie2, user);
        context.Ratings.Add(new Rating { Movie = movie, User = user, Value = 5 });

        context.SaveChanges();

        return context;
    }

    [Fact]
    public void GetFilteredMovies_ByTitle_ReturnsMatch()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var result = service.GetFilteredMovies("Matrix", null, null);

        Assert.Single(result);
    }

    [Fact]
    public void GetFilteredMovies_ByGenre_ReturnsMatch()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var result = service.GetFilteredMovies(null, null, "Drama");

        Assert.Single(result);
    }

    [Fact]
    public void GetFilteredMovies_EmptyCriteria_ReturnsEmpty()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var result = service.GetFilteredMovies("Nonexistent", 1980, "Comedy");

        Assert.Empty(result);
    }

    [Fact]
    public void GetTopRated_ReturnsSortedMovies()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var result = service.GetTopRated().ToList();

        Assert.True(result.Count <= 5);
        Assert.Contains(result, r => r.GetType().GetProperty("Title")?.GetValue(r)?.ToString()?.Contains("Matrix") == true);
    }

    [Fact]
    public void GetTopRatedByUser_ValidUser_ReturnsMovies()
    {
        var ctx = CreateDbContext();
        var userId = ctx.Users.First().Id;
        var service = new MovieService(ctx);

        var result = service.GetTopRatedByUser(userId);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetTopRatedByUser_InvalidUser_ReturnsEmpty()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var result = service.GetTopRatedByUser(-1);

        Assert.Empty(result);
    }

    [Fact]
    public void RateMovie_AddsNewRating()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var movieId = ctx.Movies.First().Id;
        var user = new User { Name = "Another User" };
        ctx.Users.Add(user);
        ctx.SaveChanges();

        service.RateMovie(user.Id, movieId, 4);

        var rating = ctx.Ratings.FirstOrDefault(r => r.UserId == user.Id && r.MovieId == movieId);
        Assert.NotNull(rating);
        Assert.Equal(4, rating!.Value);
    }

    [Fact]
    public void RateMovie_UpdatesExistingRating()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);

        var user = ctx.Users.First();
        var movie = ctx.Movies.First();

        service.RateMovie(user.Id, movie.Id, 3);

        var rating = ctx.Ratings.FirstOrDefault(r => r.UserId == user.Id && r.MovieId == movie.Id);
        Assert.NotNull(rating);
        Assert.Equal(3, rating!.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void RateMovie_InvalidRating_ThrowsException(int invalidRating)
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);
        var user = ctx.Users.First();
        var movie = ctx.Movies.First();

        var ex = Assert.Throws<ArgumentException>(() =>
            service.RateMovie(user.Id, movie.Id, invalidRating));

        Assert.Contains("Rating must be between 1 and 5", ex.Message);
    }

    [Fact]
    public void RateMovie_MovieNotFound_Throws()
    {
        var ctx = CreateDbContext();
        var service = new MovieService(ctx);
        var userId = ctx.Users.First().Id;

        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.RateMovie(userId, -999, 3));

        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
