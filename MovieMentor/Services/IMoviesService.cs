using MovieMentor.DTO;

namespace MovieMentor.Services;

public interface IMoviesService
{
    IEnumerable<MovieDto> GetAllMovies();

    MovieDto? GetMovie(int id);
}
