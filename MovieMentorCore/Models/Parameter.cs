namespace MovieMentorCore.Models;

public abstract record Parameter
{
    public record Concrete(string Value) : Parameter;

    public record Reference(int Index) : Parameter;
}
