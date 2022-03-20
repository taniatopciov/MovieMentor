using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Instance GenreDefinition(string name) => new("Genre", new ParameterList.Builder()
        .AddParameter("Name", new Parameter.SingleValue(name))
        .Build());
}
