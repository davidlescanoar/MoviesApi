namespace MoviesApi.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int YearOfRelease { get; set; }
        public int RunningTime { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
