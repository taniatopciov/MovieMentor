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
            new("Multiple", "Genre", new List<string> { "Action", "Drama", "Comedy", "Thriller", "Horror" }),
            new("Single", "Director", new List<string> { "Christopher Nolan", "Steven Spielberg" }),
            new("Multiple", "Actors", new List<string> { "Adam Sandler", "Zendaya" }),
            new("Single", "Duration",
                new List<string> { "short (< 90 min)", "medium (90 min - 120 min)", "long (> 120 min)" }),
            new("Single", "Year", new List<string> { "'80s", "'90s", "2000s", "2010s", "2020s" }),
            new("Single", "Rating", new List<string> { "> 9", "8-9", "7-8", "6-7", "< 6" }),
            new("Multiple", "Awards",
                new List<string> { "Best Picture", "Best Director", "Best Actor", "Best Screenplay" }),
            new("Single", "Country", new List<string> { "USA", "Romania", "Germany", "France", "Spain" }),
        };
    }
}
