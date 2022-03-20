using MovieMentorCore.Models;

namespace MovieMentor.Rules;

public static partial class Rules
{
    public static RuleDefinition.Instance CountryDefinition(string name) => new("Country", new ParameterList.Builder()
        .AddParameter("Name", new Parameter.SingleValue(name))
        .Build());

    public static RuleDefinition.Instance CountryInstance(Parameter name) => new("Country",
        new ParameterList.Builder()
            .AddParameter("Name", name)
            .Build());
}
