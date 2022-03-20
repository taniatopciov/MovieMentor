namespace MovieMentorCore.Models;

public abstract record RuleDefinition(string Name, ParameterList ParametersList)
{
    public record Instance(string Name, ParameterList ParametersList) : RuleDefinition(Name, ParametersList);

    public record Composite(string Name, ParameterList ParametersList, IList<Instance> Definitions) : RuleDefinition(
        Name, ParametersList);
}
