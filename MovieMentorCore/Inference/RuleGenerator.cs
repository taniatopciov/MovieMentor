using MovieMentorCore.Models;

namespace MovieMentorCore.Inference;

public class RuleGenerator
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;
    private readonly RuleValidator _ruleValidator;

    private readonly List<string[]> _possibilities = new();
    private string?[] _current = Array.Empty<string>();
    private IList<RuleDefinition> _currentDefinitions = new List<RuleDefinition>();
    private RuleInstance _targetRuleInstance = new("", new List<Parameter>());

    public RuleGenerator(IDictionary<string, IList<RuleDefinition>> allDefinitions, RuleValidator ruleValidator)
    {
        _allDefinitions = allDefinitions;
        _ruleValidator = ruleValidator;
    }

    public IList<string[]> Generate(RuleInstance ruleInstance)
    {
        _targetRuleInstance = ruleInstance;
        var (name, parameters) = ruleInstance;
        if (!_allDefinitions.TryGetValue(name, out var definitions))
        {
            return new List<string[]>();
        }

        // later rules overloading
        // definitions = definitions.Where(r => r.ParameterCount == parameters.Count).ToList();
        if (definitions.Count == 0)
        {
            return new List<string[]>();
        }

        _currentDefinitions = definitions;
        _current = new string[parameters.Count];

        Back(0);

        return _possibilities;
    }

    private void Back(int step)
    {
        if (step >= _current.Length)
        {
            return;
        }

        var possibilities = GetPossibilities(step);

        foreach (var possibility in possibilities)
        {
            _current[step] = possibility;
            if (!Valid(step))
            {
                continue;
            }

            if (Solution(step))
            {
                var copy = new string[_current.Length];
                Array.Copy(_current, copy, _current.Length);

                _possibilities.Add(copy);
            }
            else
            {
                Back(step + 1);
            }
        }
    }

    private List<string> GetPossibilities(int step)
    {
        if (_targetRuleInstance.Parameters[step] is Parameter.Concrete(var targetConcreteValue))
        {
            return new List<string> { targetConcreteValue };
        }

        var possibilities = new List<string>();
        foreach (var currentDefinition in _currentDefinitions)
        {
            switch (currentDefinition)
            {
                case RuleDefinition.Concrete(_, var parameters):
                    possibilities.Add(parameters[step].Value);
                    break;

                case RuleDefinition.Composite(_, var definitionParameters, var ruleInstances):
                    switch (definitionParameters[step])
                    {
                        case Parameter.Concrete(var value):
                            possibilities.Add(value);
                            break;
                        case Parameter.Reference(var index):
                        {
                            var ruleGenerator = new RuleGenerator(_allDefinitions, _ruleValidator);
                            var possibilitiesSet = new HashSet<string>();

                            foreach (var ruleInstance in ruleInstances)
                            {
                                if (!ruleInstance.Parameters.Any(parameter =>
                                        parameter is Parameter.Reference(var refIndex) && refIndex == index))
                                {
                                    continue;
                                }

                                foreach (var concreteParameters in ruleGenerator.Generate(ruleInstance))
                                {
                                    for (var i = 0; i < ruleInstance.Parameters.Count; i++)
                                    {
                                        var param = ruleInstance.Parameters[i];
                                        if (param is Parameter.Reference(var refIndex) && refIndex == index)
                                        {
                                            possibilitiesSet.Add(concreteParameters[i]);
                                        }
                                    }
                                }
                            }

                            possibilities.AddRange(possibilitiesSet);

                            break;
                        }
                    }

                    break;
            }
        }

        return possibilities;
    }

    private bool Valid(int step)
    {
        return step < _current.Length && _current[step] != null;
    }

    private bool Solution(int step)
    {
        if (step != _current.Length - 1)
        {
            return false;
        }

        var parameters = _current.Select(value => new Parameter.Concrete(value ?? "")).Cast<Parameter>().ToList();
        return _ruleValidator.Validate(new RuleInstance(_targetRuleInstance.Name, parameters));
    }
}
