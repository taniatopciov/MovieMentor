namespace MovieMentor.DTO;

public record RecommendationDto(List<Choice> choices);

public record Choice(string key, List<string> values);
