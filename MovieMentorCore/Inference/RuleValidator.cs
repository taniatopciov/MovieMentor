using MovieMentorCore.Models;

namespace MovieMentorCore.Inference;

public class RuleValidator
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;

    public RuleValidator(IDictionary<string, IList<RuleDefinition>> allDefinitions)
    {
        _allDefinitions = allDefinitions;
    }

    public bool Validate(RuleDefinition.Instance ruleInstance)
    {
        var (name, parameterList) = ruleInstance;
        if (!_allDefinitions.TryGetValue(name, out var definitions))
        {
            return false;
        }

        if (parameterList.HasReferences)
        {
            return false;
        }

        foreach (var definition in definitions)
        {
            if (definition.ParametersList.Count != parameterList.Count)
            {
                continue;
            }

            switch (definition)
            {
                case RuleDefinition.Composite(var definitionName, var definitionParameters, var definitionInstances)
                    when definitionName == name:
                {
                    var allMatch = true;

                    // foreach definition instance
                    // replace the reference parameters of the original rule instance
                    // with the provided parameters
                    foreach (var (instanceName, instanceParameterList) in definitionInstances)
                    {
                        var updatedParameterListBuilder = new ParameterList.Builder();

                        // replace instanceParameterList reference parameters with the provided parameters

                        foreach (var (parameterName, parameter) in instanceParameterList.Parameters)
                        {
                            if (parameter is Parameter.Reference(var index))
                            {
                                // search in definition for reference parameter with index
                                var definitionReferenceParameterName = definitionParameters.Parameters
                                    .FirstOrDefault(pair =>
                                        pair.Value is Parameter.Reference(var refIndex) && refIndex == index).Key;
                                if (definitionReferenceParameterName == default) // it can be null
                                {
                                    updatedParameterListBuilder =
                                        updatedParameterListBuilder.AddParameter(parameterName,
                                            new Parameter.DontCare());
                                    continue;
                                }

                                // search for concrete parameter in provided rule instance
                                var concreteParameter = parameterList[definitionReferenceParameterName];
                                if (concreteParameter == null)
                                {
                                    // allMatch = false;
                                    // break;
                                    updatedParameterListBuilder =
                                        updatedParameterListBuilder.AddParameter(parameterName,
                                            new Parameter.DontCare());
                                    continue;
                                }

                                updatedParameterListBuilder =
                                    updatedParameterListBuilder.AddParameter(parameterName, concreteParameter);
                            }
                            else
                            {
                                updatedParameterListBuilder =
                                    updatedParameterListBuilder.AddParameter(parameterName, parameter);
                            }
                        }

                        if (!allMatch)
                        {
                            break;
                        }

                        var updatedDefinitionInstance =
                            new RuleDefinition.Instance(instanceName, updatedParameterListBuilder.Build());

                        if (!Validate(updatedDefinitionInstance))
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

                case RuleDefinition.Instance(var definitionName, var concreteParametersList)
                    when definitionName == name:
                {
                    var allMatch = true;

                    foreach (var (parameterName, parameter) in parameterList.Parameters)
                    {
                        var concreteParam = concreteParametersList[parameterName];
                        if (concreteParam == null)
                        {
                            allMatch = false;
                            break;
                        }

                        switch (parameter)
                        {
                            case Parameter.DontCare:
                                break;
                            case Parameter.MultipleValues(var values):
                                if (concreteParam is not Parameter.MultipleValues(var concreteValues))
                                {
                                    allMatch = false;
                                    break;
                                }

                                if (values.Count != concreteValues.Count)
                                {
                                    allMatch = false;
                                    break;
                                }

                                if (concreteValues.Any(value => !values.Contains(value)))
                                {
                                    allMatch = false;
                                    // add break if conditions are added
                                }

                                break;

                            case Parameter.SingleValue(var value):
                                if (concreteParam is not Parameter.SingleValue(var concreteValue))
                                {
                                    allMatch = false;
                                    break;
                                }

                                if (value != concreteValue)
                                {
                                    allMatch = false;
                                    // add break if conditions are added
                                }

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
