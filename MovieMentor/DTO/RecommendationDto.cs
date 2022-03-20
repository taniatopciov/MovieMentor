namespace MovieMentor.DTO;

public record RecommendationDto(List<Choice> choices)
{
    public IEnumerable<string>? GetValues(string key)
    {
        var choice = choices.FirstOrDefault(c => c.key == key);

        return choice?.values;
    }

    public string? GetSingleValue(string key)
    {
        var values = GetValues(key);

        return values?.FirstOrDefault();
    }
}

public record Choice(string key, List<string> values);
