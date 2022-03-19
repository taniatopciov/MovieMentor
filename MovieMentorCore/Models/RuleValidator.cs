namespace MovieMentorCore.Models;

public class RuleValidator
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;

    public RuleValidator(IDictionary<string, IList<RuleDefinition>> allDefinitions)
    {
        _allDefinitions = allDefinitions;
    }

    public bool Validate(RuleInstance ruleInstance)
    {
        var (name, parameters) = ruleInstance;
        if (!_allDefinitions.TryGetValue(name, out var definitions))
        {
            return false;
        }

        if (parameters.Any(param => param is Parameter.Reference))
        {
            return false;
        }

        foreach (var definition in definitions)
        {
            if (definition.ParameterCount != parameters.Count)
            {
                continue;
            }

            switch (definition)
            {
                case RuleDefinition.Composite(var definitionName, var definitionParameters, var definitionInstances)
                    when definitionName == name:
                {
                    var allMatch = true;

                    foreach (var (instanceName, instanceParameters) in definitionInstances)
                    {
                        IList<Parameter> updatedParams = new List<Parameter>();
                        foreach (var instanceParameter in instanceParameters)
                        {
                            if (instanceParameter is Parameter.Reference(var index))
                            {
                                var indexOfConcreteParam =
                                    definitionParameters.IndexOf(p => p is Parameter.Reference(var i) && i == index);
                                if (indexOfConcreteParam == -1)
                                {
                                    return false;
                                }

                                updatedParams.Add(parameters[indexOfConcreteParam]);
                            }
                            else
                            {
                                updatedParams.Add(instanceParameter);
                            }
                        }

                        var updatedDefinitionInstance = new RuleInstance(instanceName, updatedParams);

                        if (!Validate(updatedDefinitionInstance))
                        {
                            return false;
                        }
                    }

                    if (allMatch)
                    {
                        return true;
                    }
                }

                    break;
                case RuleDefinition.Concrete(var definitionName, var concreteParameters) when definitionName == name:
                {
                    var allMatch = true;
                    for (var i = 0; i < parameters.Count; i++)
                    {
                        var param = parameters[i] as Parameter.Concrete;
                        if (concreteParameters[i].Value != param?.Value)
                        {
                            allMatch = false;
                            break;
                        }
                    }

                    if (allMatch)
                    {
                        return true;
                    }
                }
                    break;
            }
        }

        return false;
    }
}
