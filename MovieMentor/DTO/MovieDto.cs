namespace MovieMentor.DTO;

public record MovieDto(int Id, string Title, List<string> Genres, List<DirectorDto> Directors, List<ActorDto> Actors,
    int Year, List<string> Awards, int Duration, string Country, double Rating);
