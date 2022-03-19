namespace MovieMentor.Data;

public enum SourceType
{
    Database,
    Rule,
}

public enum ValueType
{
    Single,
    Multiple
}

public class Tag
{
    public int ID { get; set; }

    public string Name { get; set; }

    public SourceType SourceType { get; set; }

    public string SourceName { get; set; }
    
    public string FieldName { get; set; }

    public ValueType ValueType { get; set; }
}
