namespace MovieMentor.DTO;

public record MovieDto(int Id, string Title, List<string> Genres, List<DirectorDto> Directors, List<ActorDto> Actors,
    int Year, int Duration, string Country, string Rating, string Link, string ImageLink, string Description);