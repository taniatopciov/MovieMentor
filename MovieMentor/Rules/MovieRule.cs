using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Instance MovieDefinition(string id, string title, string year, string rating, string duration) =>
        new("Movie", new ParameterList.Builder()
            .AddParameter("ID", new Parameter.SingleValue(id))
            .AddParameter("Title", new Parameter.SingleValue(title))
            .AddParameter("Year", new Parameter.SingleValue(year))
            .AddParameter("Rating", new Parameter.SingleValue(rating))
            .AddParameter("Duration", new Parameter.SingleValue(duration))
            .Build());

    public static RuleDefinition.Instance MovieRuleInstance(Parameter id, Parameter title, Parameter year, Parameter rating, Parameter duration) =>
        new("Movie", new ParameterList.Builder()
            .AddParameter("ID", id)
            .AddParameter("Title", title)
            .AddParameter("Year", year)
            .AddParameter("Rating", rating)
            .AddParameter("Duration", duration)
            .Build());
}
