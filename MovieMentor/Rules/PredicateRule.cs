namespace MovieMentor.Rules;

public class PredicateRule
{
    private record PredicateChoice(Predicate<int> Predicate, string Label);

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

    public string Evaluate(int value)
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
        private readonly PredicateRule _predicateRule;

        public Builder(string name, string defaultLabel)
        {
            _predicateRule = new PredicateRule(name, defaultLabel);
        }

        public Builder AddChoice(string label, Predicate<int> predicate)
        {
            _predicateRule._predicateChoices.Add(new PredicateChoice(predicate, label));

            return this;
        }


        public PredicateRule Build()
        {
            return _predicateRule;
        }
    }

// "short (< 90 min)", "medium (90 min - 120 min)", "long (> 120 min)"
}
