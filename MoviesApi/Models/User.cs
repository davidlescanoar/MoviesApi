namespace MoviesApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
