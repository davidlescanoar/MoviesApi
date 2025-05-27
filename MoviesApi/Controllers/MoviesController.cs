using Microsoft.AspNetCore.Mvc;
using MoviesApi.Data;
using MoviesApi.Services.Interfaces;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesDbContext _context;
        private readonly IMovieService _movieService;

        public MoviesController(MoviesDbContext context, IMovieService movieService)
        {
            _context = context;
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult GetFiltered([FromQuery] string? title, [FromQuery] int? year, [FromQuery] string? genre)
        {
            if (title == null && year == null && genre == null)
                return BadRequest();

            var result = _movieService.GetFilteredMovies(title, year, genre);
            return result.Any() ? Ok(result) : NotFound();
        }

        [HttpGet("top-rated")]
        public IActionResult GetTopRated() => Ok(_movieService.GetTopRated());

        [HttpGet("top-rated/{userId}")]
        public IActionResult GetTopRatedByUser(int userId)
        {
            var result = _movieService.GetTopRatedByUser(userId);
            return result.Any() ? Ok(result) : NotFound();
        }

        [HttpPost("rate")]
        public IActionResult Rate([FromQuery] int userId, [FromQuery] int movieId, [FromQuery] int rating)
        {
            try
            {
                _movieService.RateMovie(userId, movieId, rating);
                return Ok();
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return NotFound(ex.Message); }
        }
    }
}
