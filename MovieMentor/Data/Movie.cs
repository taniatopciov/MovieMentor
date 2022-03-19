namespace MovieMentor.Data;

public class Movie
{
    public int ID { get; set; }

    public string Title { get; set; }

    public ICollection<Genre> Genres { get; set; }

    public ICollection<Director> Directors { get; set; }

    public ICollection<Actor> Actors { get; set; }
    
    public int Year { get; set; }
    
    public ICollection<Award> Awards { get; set; }
    
    public int Duration { get; set; }
    
    public Country Country { get; set; }
    
    public double Rating { get; set; }
}
