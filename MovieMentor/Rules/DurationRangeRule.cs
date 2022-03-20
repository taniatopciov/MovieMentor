using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Instance DurationRangeDefinition(string type, string value) => new("DurationRange",
        new ParameterList.Builder()
            .AddParameter("Name", new Parameter.SingleValue(type))
            .AddParameter("Value", new Parameter.SingleValue(value))
            .Build());

    public static RuleDefinition.Instance DurationRangeInstance(Parameter type, Parameter value) => new("DurationRange",
        new ParameterList.Builder()
            .AddParameter("Name", type)
            .AddParameter("Value", value)
            .Build());
}
