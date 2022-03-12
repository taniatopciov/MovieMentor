using Microsoft.AspNetCore.Mvc;
using MovieMentor.Models;

namespace MovieMentor.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    [HttpGet("/tags")]
    public IEnumerable<Tag> GetTags()
    {
        return new List<Tag>
        {
            new("Single", "Year", new List<string> { "'80s", "'90s", "2000s", "2010s", "2020s" }),
            new("Multiple", "Category", new List<string> { "Action", "Drama", "Comedy", "Thriller", "Horror" }),
        };
    }
}
