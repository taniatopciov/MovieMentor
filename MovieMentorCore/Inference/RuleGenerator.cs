using MovieMentorCore.Models;

namespace MovieMentorCore.Inference;

public class RuleGenerator
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;
    private readonly RuleValidator _ruleValidator;

    private readonly List<ParameterList> _possibilities = new();
    private readonly IList<IList<Parameter>> _parameterPossibilities = new List<IList<Parameter>>();

    private ParameterList _current = ParameterList.Empty();
    private IList<RuleDefinition> _currentDefinitions = new List<RuleDefinition>();
    private RuleDefinition.Instance _targetRuleInstance = new("", ParameterList.Empty());
    private IList<string> _targetParameterNames = Array.Empty<string>();

    public RuleGenerator(IDictionary<string, IList<RuleDefinition>> allDefinitions, RuleValidator ruleValidator)
    {
        _allDefinitions = allDefinitions;
        _ruleValidator = ruleValidator;
    }

    public IList<ParameterList> Generate(RuleDefinition.Instance ruleInstance)
    {
        _targetRuleInstance = ruleInstance;
        var (name, parameters) = ruleInstance;
        if (!_allDefinitions.TryGetValue(name, out var definitions))
        {
            return new List<ParameterList>();
        }

        // later rules overloading
        // definitions = definitions.Where(r => r.ParameterCount == parameters.Count).ToList();
        if (definitions.Count == 0 || parameters.Count == 0)
        {
            return new List<ParameterList>();
        }

        _currentDefinitions = definitions;
        _current = parameters;
        _targetParameterNames = parameters.Parameters.Keys.ToList();

        for (var i = 0; i < parameters.Count; i++)
        {
            _parameterPossibilities.Add(GetPossibilities(i));
        }

        Back(0);

        return _possibilities;
    }

    private void Back(int step)
    {
        if (step >= _current.Count)
        {
            return;
        }

        foreach (var possibility in _parameterPossibilities[step])
        {
            _current[_targetParameterNames[step]] = possibility;
            if (!Valid(step))
            {
                continue;
            }

            if (Solution(step))
            {
                _possibilities.Add(_current.Copy());
            }
            else
            {
                Back(step + 1);
            }
        }
    }

    private List<Parameter> GetPossibilities(int step)
    {
        var stepParameterName = _targetParameterNames[step];
        var stepParameter = _targetRuleInstance.ParametersList[stepParameterName];
        if (stepParameter is Parameter.SingleValue or Parameter.MultipleValues)
        {
            return new List<Parameter>
            {
                stepParameter
            };
        }

        var possibilities = new List<Parameter>();
        foreach (var currentDefinition in _currentDefinitions)
        {
            switch (currentDefinition)
            {
                case RuleDefinition.Instance(_, var parameters):

                    var currentParameter = parameters[stepParameterName];
                    if (currentParameter == null)
                    {
                        continue;
                    }

                    if (currentParameter is Parameter.SingleValue or Parameter.MultipleValues)
                    {
                        possibilities.Add(currentParameter);
                    }

                    break;

                case RuleDefinition.Composite(_, var definitionParameters, var ruleInstances):
                    var definitionParameter = definitionParameters[stepParameterName];

                    switch (definitionParameter)
                    {
                        case Parameter.SingleValue:
                        case Parameter.MultipleValues:
                            possibilities.Add(definitionParameter);
                            break;

                        case Parameter.Reference(var index):
                        {
                            var possibilitiesSetList = new List<HashSet<Parameter>>();

                            foreach (var (name, parametersList) in ruleInstances)
                            {
                                if (!_allDefinitions.TryGetValue(name, out var ruleInstanceDefinitions))
                                {
                                    continue;
                                }

                                // todo check if missing nested reference values
                                var desiredParameterNames = parametersList.Parameters
                                    .Where(pair => pair.Value is Parameter.Reference(var refIndex) && refIndex == index)
                                    .Select(pair => pair.Key)
                                    .ToList();

                                foreach (var parameterName in desiredParameterNames)
                                {
                                    var possibilitiesSet = new HashSet<Parameter>();
                                    foreach (var ruleDefinition in ruleInstanceDefinitions)
                                    {
                                        var parameter = ruleDefinition.ParametersList[parameterName];
                                        if (parameter is Parameter.SingleValue or Parameter.MultipleValues)
                                        {
                                            possibilitiesSet.Add(parameter);
                                        }
                                    }

                                    possibilitiesSetList.Add(possibilitiesSet);
                                }
                            }

                            if (possibilitiesSetList.Count > 0)
                            {
                                var intersectedPossibilitiesSet = possibilitiesSetList
                                    .Skip(1)
                                    .Aggregate(new HashSet<Parameter>(possibilitiesSetList.First()),
                                        (h, e) =>
                                        {
                                            h.IntersectWith(e);
                                            return h;
                                        });
                                possibilities.AddRange(intersectedPossibilitiesSet);
                            }

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
        return step < _current.Count;
    }

    private bool Solution(int step)
    {
        if (step != _current.Count - 1)
        {
            return false;
        }

        return _ruleValidator.Validate(new RuleDefinition.Instance(_targetRuleInstance.Name, _current));
    }
}
