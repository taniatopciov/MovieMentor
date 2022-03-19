namespace MovieMentor.Data;

public class Movie
{
    public int ID { get; set; }

    public string Title { get; set; }

    public List<Genre> Genres { get; set; }
    
    public List<Director> Directors { get; set; }
    
    public List<Actor> Actors { get; set; }
    
    public int Year { get; set; }
    
    public List<Award> Awards { get; set; }
    
    public int Duration { get; set; }
    
    public Country Country { get; set; }
    
    public double Rating { get; set; }
}
