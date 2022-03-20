using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Instance YearRangeDefinition(string name, string value) => new("YearRange",
        new ParameterList.Builder()
            .AddParameter("Name", new Parameter.SingleValue(name))
            .AddParameter("Value", new Parameter.SingleValue(value))
            .Build());

    public static RuleDefinition.Instance YearRangeInstance(Parameter name, Parameter value) => new("YearRange",
        new ParameterList.Builder()
            .AddParameter("Name", name)
            .AddParameter("Value", value)
            .Build());
}
