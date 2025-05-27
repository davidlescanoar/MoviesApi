using Microsoft.AspNetCore.Mvc;
using MoviesApi.Data;

namespace MoviesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesDbContext _context;

        public MoviesController(MoviesDbContext context)
        {
            _context = context;
        }
    }
}
