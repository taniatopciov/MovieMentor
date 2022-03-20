namespace MovieMentorCore.Models;

public abstract record Parameter
{
    public record Reference(int Index) : Parameter;

    public record SingleValue(string Value) : Parameter;

    public record MultipleValues(ISet<string> Values) : Parameter;

    public record DontCare : Parameter;
}
