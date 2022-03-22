namespace MovieMentorCore.Data;

public class Director
{
    public int ID { get; set; }
    
    public string Name { get; set; }

    protected virtual List<Movie> Movies { get; set; }
}
