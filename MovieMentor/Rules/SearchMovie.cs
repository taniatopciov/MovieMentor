using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Composite SearchMovieDefinition() => new("SearchMovie",
        new ParameterList.Builder()
            .AddParameter("ID", new Parameter.Reference(0))
            .AddParameter("Title", new Parameter.Reference(1))
            .AddParameter("Year", new Parameter.Reference(2))
            .AddParameter("Rating", new Parameter.Reference(3))
            .AddParameter("Duration", new Parameter.Reference(4))
            .AddParameter("DurationType", new Parameter.Reference(5))
            .Build()
        , new List<RuleDefinition.Instance>
        {
            MovieRuleInstance(
                new Parameter.Reference(0),
                new Parameter.Reference(1),
                new Parameter.Reference(2),
                new Parameter.Reference(3),
                new Parameter.Reference(4)),
            DurationRangeInstance(
                new Parameter.Reference(5),
                new Parameter.Reference(4)),
        });

    public static RuleDefinition.Instance SearchMovieInstance(Parameter id, Parameter title, Parameter year,
        Parameter rating, Parameter duration, Parameter durationType) => new("SearchMovie",
        new ParameterList.Builder()
            .AddParameter("ID", id)
            .AddParameter("Title", title)
            .AddParameter("Year", year)
            .AddParameter("Rating", rating)
            .AddParameter("Duration", duration)
            .AddParameter("DurationType", durationType)
            .Build());
}
