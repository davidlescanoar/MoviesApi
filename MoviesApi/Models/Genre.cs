namespace MoviesApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
