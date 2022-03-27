namespace MovieMentor.DTO;

public record ChoiceDto(string Type, string Name, bool optional, List<string> Values);