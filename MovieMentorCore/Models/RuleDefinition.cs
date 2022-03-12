namespace MovieMentorCore.Models;

public abstract record RuleDefinition(string Name)
{
    public abstract int ParameterCount { get; }

    public record Concrete(string Name, IList<Parameter.Concrete> Parameters) : RuleDefinition(Name)
    {
        public override int ParameterCount => Parameters.Count;
    }

    public record Composite
        (string Name, IList<Parameter> Parameters, IList<RuleInstance> Definitions) : RuleDefinition(Name)
    {
        public override int ParameterCount => Parameters.Count;
    }
}
