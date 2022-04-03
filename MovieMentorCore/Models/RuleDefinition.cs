namespace MovieMentorCore.Models;

public abstract record RuleDefinition(string Name, ParameterList ParametersList)
{
    public record Instance(string Name, ParameterList ParametersList) : RuleDefinition(Name, ParametersList);

    public record Composite(Instance Rule, IList<Instance> Definitions) : RuleDefinition(Rule.Name, Rule.ParametersList);
}
