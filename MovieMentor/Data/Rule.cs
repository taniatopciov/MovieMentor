namespace MovieMentor.Data;

public class Rule
{
    public int ID { get; set; }

    public string Name { get; set; }

    public ICollection<string> Parameters { get; set; }

    public ICollection<RuleInstance> Definitions { get; set; }
}
