namespace MovieMentorCore.Data;

public class Movie
{
    public int ID { get; set; }

    public string Title { get; set; }

    public virtual List<Genre> Genres { get; set; }

    public virtual List<Director> Directors { get; set; }

    public virtual List<Actor> Actors { get; set; }

    public int Year { get; set; }

    public int Duration { get; set; }

    public virtual Country Country { get; set; }

    public string Rating { get; set; }
    
    public string Link { get; set; }
}