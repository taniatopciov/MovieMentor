using MovieMentorCore.Models;

namespace MovieMentorCore.Inference;

public class InferenceMachine
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;

    public InferenceMachine(IDictionary<string, IList<RuleDefinition>> allDefinitions)
    {
        _allDefinitions = allDefinitions;
    }

    public void AddRuleDefinition(string ruleName, RuleDefinition definition)
    {
        if (_allDefinitions.TryGetValue(ruleName, out var definitions))
        {
            definitions.Add(definition);
        }
        else
        {
            _allDefinitions.Add(ruleName, new List<RuleDefinition> { definition });
        }
    }

    public IList<ParameterList> Infer(RuleDefinition.Instance ruleInstance)
    {
        var ruleGenerator = new RuleGenerator(_allDefinitions, new RuleValidator(_allDefinitions));

        return ruleGenerator.Generate(ruleInstance);
    }
}
