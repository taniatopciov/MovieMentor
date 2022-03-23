namespace MovieMentor.Services;

public class PredicateRule<T>
{
    private record PredicateChoice(Predicate<T> Predicate, string Label);

    private readonly IList<PredicateChoice> _predicateChoices = new List<PredicateChoice>();
    private readonly string _defaultLabel;

    public string Name { get; }

    private PredicateRule(string name, string defaultLabel)
    {
        Name = name;
        _defaultLabel = defaultLabel;
    }

    public List<string> GetLabels()
    {
        var labels = new List<string>();

        foreach (var (_, label) in _predicateChoices)
        {
            labels.Add(label);
        }

        labels.Add(_defaultLabel);

        return labels;
    }

    public string Evaluate(T value)
    {
        foreach (var (predicate, label) in _predicateChoices)
        {
            if (predicate(value))
            {
                return label;
            }
        }

        return _defaultLabel;
    }

    public class Builder
    {
        private readonly PredicateRule<T> _predicateRule;

        public Builder(string name, string defaultLabel)
        {
            _predicateRule = new PredicateRule<T>(name, defaultLabel);
        }

        public Builder AddChoice(string label, Predicate<T> predicate)
        {
            _predicateRule._predicateChoices.Add(new PredicateChoice(predicate, label));

            return this;
        }


        public PredicateRule<T> Build()
        {
            return _predicateRule;
        }
    }
}
