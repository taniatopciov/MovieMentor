namespace MovieMentor.Data;

public class RuleInstance
{
    public int ID { get; set; }

    public string Name { get; set; }

    public ICollection<string> Parameters { get; set; }
}
