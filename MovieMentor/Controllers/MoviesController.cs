using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.Data;
using MovieMentor.DTO;

namespace MovieMentor.Controllers;

public static class MoviesController
{
    public static IEnumerable<MovieDto> GetMovies([FromServices] MovieContext movieContext)
    {
        return movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(m => Convert(m)).ToList();
    }

    public static MovieDto? GetMovie(int id, [FromServices] MovieContext movieContext)
    {
        var movie = movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Awards))
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .FirstOrDefault();

        return movie == null ? null : Convert(movie);
    }

    public static IEnumerable<TagDto> GetTags()
    {
        return new List<TagDto>
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

    private static MovieDto Convert(Movie movie)
    {
        return new MovieDto(movie.ID, movie.Title, movie.Genres.Select(g => g.Name).ToList(),
            movie.Directors.Select(d => new DirectorDto(d.Name)).ToList(),
            movie.Actors.Select(a => new ActorDto(a.Name, a.Country.Name)).ToList(), movie.Year,
            movie.Awards.Select(a => a.Name).ToList(), movie.Duration, movie.Country.Name, movie.Rating);
    }
}
