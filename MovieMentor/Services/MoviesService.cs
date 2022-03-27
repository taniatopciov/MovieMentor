using Microsoft.EntityFrameworkCore;
using MovieMentor.DAL;
using MovieMentor.DTO;
using MovieMentorCore.Data;

namespace MovieMentor.Services;

public class MoviesService : IMoviesService
{
    private readonly MovieContext _movieContext;

    public MoviesService(MovieContext movieContext)
    {
        _movieContext = movieContext;
    }

    public IEnumerable<MovieDto> GetAllMovies()
    {
        return _movieContext.Movies
            .Include(movie => movie.Actors).ThenInclude(a => a.Country)
            .Include(nameof(Movie.Country))
            .Include(nameof(Movie.Directors))
            .Include(nameof(Movie.Genres))
            .Select(movie => ConvertMovie(movie));
    }

    public MovieDto? GetMovie(int id)
    {
        return GetAllMovies()
            .FirstOrDefault(m => m.Id == id);
    }

    private static MovieDto ConvertMovie(Movie movie)
    {
        return new MovieDto(movie.ID, movie.Title, movie.Genres.Select(g => g.Name).ToList(),
            movie.Directors.Select(d => new DirectorDto(d.Name)).ToList(),
            movie.Actors.Select(a => new ActorDto(a.Name, a.Country.Name)).ToList(), movie.Year,
            movie.Duration, movie.Country.Name, movie.Rating);
    }
}