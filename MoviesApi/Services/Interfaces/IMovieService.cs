namespace MoviesApi.Services.Interfaces
{
    public interface IMovieService
    {
        IEnumerable<object> GetFilteredMovies(string? title, int? year, string? genre);
        IEnumerable<object> GetTopRated();
        IEnumerable<object> GetTopRatedByUser(int userId);
        void RateMovie(int userId, int movieId, int rating);
    }
}
