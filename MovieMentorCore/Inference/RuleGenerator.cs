using MovieMentorCore.Models;

namespace MovieMentorCore.Inference;

public class RuleGenerator
{
    private readonly IDictionary<string, IList<RuleDefinition>> _allDefinitions;
    private readonly RuleValidator _ruleValidator;

    public RuleGenerator(IDictionary<string, IList<RuleDefinition>> allDefinitions, RuleValidator ruleValidator)
    {
        _allDefinitions = allDefinitions;
        _ruleValidator = ruleValidator;
    }

    public IList<ParameterList> Generate(RuleDefinition.Instance ruleInstance)
    {
        var (name, parameterList) = ruleInstance;

        if (!parameterList.HasReferences)
        {
            return _ruleValidator.Validate(ruleInstance)
                ? new List<ParameterList> { parameterList }
                : new List<ParameterList>();
        }

        if (!_allDefinitions.TryGetValue(name, out var definitions))
        {
            return new List<ParameterList>();
        }

        // later rules overloading
        // definitions = definitions.Where(r => r.ParameterCount == parameters.Count).ToList();
        if (definitions.Count == 0 || parameterList.Count == 0)
        {
            return new List<ParameterList>();
        }

        return GenerateConcreteInstances(ruleInstance)
            .Select(r => r.ParametersList)
            .ToList();
    }

    private IEnumerable<RuleDefinition.Instance> GenerateConcreteInstances(RuleDefinition.Instance inputRuleInstance)
    {
        if (!inputRuleInstance.ParametersList.HasReferences)
        {
            if (_ruleValidator.Validate(inputRuleInstance))
            {
                yield return inputRuleInstance;
            }

            yield break;
        }

        if (!_allDefinitions.TryGetValue(inputRuleInstance.Name, out var definitions))
        {
            yield break;
        }

        foreach (var definition in definitions)
        {
            switch (definition)
            {
                case RuleDefinition.Instance instance:
                    if (!instance.ParametersList.HasReferences)
                    {
                        var concreteInstance = SetReferenceParametersOfInstance(inputRuleInstance, instance);
                        if (concreteInstance != null)
                        {
                            yield return concreteInstance;
                        }
                    }

                    break;

                case RuleDefinition.Composite(var instance, var instanceDefinitions):

                    var backtracking = new Backtracking<RuleDefinition.Instance>(instanceDefinitions.Count,
                        (step, current) =>
                        {
                            if (step >= current.Length)
                            {
                                return false;
                            }

                            return EnsureAllReferenceParametersWithTheSameIndexAreEqual(instanceDefinitions, step,
                                current);
                        },
                        (step, current) => step == current.Length - 1,
                        step =>
                        {
                            // based on the input rule, validate the rule parameters with the provided concrete parameters

                            var generateConcreteInstances = GenerateConcreteInstances(instanceDefinitions[step]);
                            return generateConcreteInstances
                                .Where(generatedDefinition =>
                                {
                                    foreach (var (generatedParameterName, generatedParameter) in generatedDefinition
                                                 .ParametersList)
                                    {
                                        switch (instanceDefinitions[step].ParametersList[generatedParameterName])
                                        {
                                            case null:
                                                return false;
                                            case Parameter.SingleValue(var value):
                                            {
                                                if (generatedParameter is not Parameter.SingleValue(var generatedValue))
                                                {
                                                    return false;
                                                }

                                                if (value != generatedValue)
                                                {
                                                    return false;
                                                }

                                                break;
                                            }
                                            case Parameter.MultipleValues(var values):
                                            {
                                                if (generatedParameter is not Parameter.MultipleValues(var
                                                    generatedValues))
                                                {
                                                    return false;
                                                }

                                                if (values.Any() && generatedValues.Any() &&
                                                    !values.Intersect(generatedValues).Any())
                                                {
                                                    return false;
                                                }

                                                break;
                                            }
                                            case Parameter.Reference(var index):
                                            {
                                                var instanceParameterName = instance.ParametersList.FirstOrDefault(p =>
                                                    p.Value is Parameter.Reference(var instanceIndex) &&
                                                    instanceIndex == index).Key;

                                                if (instanceParameterName != default) // it can happen
                                                {
                                                    var providedParameter =
                                                        inputRuleInstance.ParametersList[instanceParameterName];
                                                    switch (providedParameter)
                                                    {
                                                        case Parameter.SingleValue(var providedValue):
                                                        {
                                                            if (generatedParameter is not Parameter.SingleValue(var
                                                                generatedValue))
                                                            {
                                                                return false;
                                                            }

                                                            if (providedValue != generatedValue)
                                                            {
                                                                return false;
                                                            }

                                                            break;
                                                        }
                                                        case Parameter.MultipleValues(var providedValues):
                                                        {
                                                            if (generatedParameter is not Parameter.MultipleValues(var
                                                                generatedValues))
                                                            {
                                                                return false;
                                                            }

                                                            if (providedValues.Any() && generatedValues.Any() &&
                                                                !providedValues.Intersect(generatedValues).Any())
                                                            {
                                                                return false;
                                                            }

                                                            break;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                        }
                                    }

                                    return true;
                                });
                        });

                    foreach (var instanceDefinitionsPossibilities in backtracking.GetSolutions())
                    {
                        // construct concrete the rule based on the definitions

                        var (instanceName, instanceParameterList) = instance;

                        var parameterListBuilder = new ParameterList.Builder();

                        foreach (var (parameterName, parameter) in instanceParameterList)
                        {
                            if (parameter is Parameter.Reference(var index))
                            {
                                // search for value in instanceDefinitionsPossibilities
                                Parameter? concreteParameter = null;
                                for (var i = 0; i < instanceDefinitionsPossibilities.Length; i++)
                                {
                                    foreach (var pair in instanceDefinitions[i].ParametersList)
                                    {
                                        if (pair.Value is Parameter.Reference(var refIndex) && refIndex == index)
                                        {
                                            concreteParameter = instanceDefinitionsPossibilities[i]
                                                .ParametersList[pair.Key];
                                            if (concreteParameter != null)
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    if (concreteParameter != null)
                                    {
                                        break;
                                    }
                                }

                                if (concreteParameter != null)
                                {
                                    parameterListBuilder.AddParameter(parameterName, concreteParameter);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                parameterListBuilder.AddParameter(parameterName, parameter);
                            }
                        }

                        var parametersList = parameterListBuilder.Build();
                        if (parametersList.Count == instanceParameterList.Count)
                        {
                            var updatedInstance = new RuleDefinition.Instance(instanceName, parametersList);
                            yield return updatedInstance;
                        }
                    }

                    break;
            }
        }
    }

    private bool EnsureAllReferenceParametersWithTheSameIndexAreEqual(
        IList<RuleDefinition.Instance> instanceDefinitions, int step, RuleDefinition.Instance[] instancePossibilities)
    {
        var possibilityFromInstance =
            SetReferenceParametersOfInstance(instanceDefinitions[step], instancePossibilities[step]);
        if (possibilityFromInstance == null)
        {
            return false;
        }

        foreach (var (instanceParameterName, instanceParameter) in instanceDefinitions[step].ParametersList)
        {
            if (instanceParameter is not Parameter.Reference reference)
            {
                continue;
            }

            var concreteParameter = instancePossibilities[step].ParametersList[instanceParameterName];
            if (concreteParameter == null)
            {
                return false;
            }

            for (var i = 0; i < step; i++)
            {
                foreach (var (previousParameterName, previousParameter) in instanceDefinitions[i].ParametersList)
                {
                    if (previousParameter is not Parameter.Reference(var refIndex) ||
                        reference.Index != refIndex)
                    {
                        continue;
                    }

                    var concretePreviousParameter =
                        instancePossibilities[i].ParametersList[previousParameterName];
                    if (concretePreviousParameter == null)
                    {
                        return false;
                    }

                    if (concreteParameter is Parameter.SingleValue(var value) &&
                        concretePreviousParameter is Parameter.SingleValue(var previousValue) &&
                        value == previousValue)
                    {
                        continue;
                    }

                    if (concreteParameter is Parameter.MultipleValues(var values) &&
                        concretePreviousParameter is Parameter.MultipleValues(var previousValues) &&
                        values.Any(v => previousValues.Contains(v)))
                    {
                        continue;
                    }

                    return false;
                }
            }
        }

        return true;
    }

    private RuleDefinition.Instance? SetReferenceParametersOfInstance(RuleDefinition.Instance inputRuleInstance,
        RuleDefinition.Instance instance)
    {
        var (instanceName, instanceParameterList) = instance;

        var parameterListBuilder = new ParameterList.Builder();

        // iterate over all input parameters
        // if the parameter is reference, replace it with the concrete version
        // if the parameter is concrete, use it
        foreach (var (parameterName, parameter) in inputRuleInstance.ParametersList)
        {
            if (parameter is Parameter.Reference)
            {
                var concreteParameter = instanceParameterList[parameterName];
                if (concreteParameter != null)
                {
                    parameterListBuilder.AddParameter(parameterName, concreteParameter);
                }
            }
            else
            {
                parameterListBuilder.AddParameter(parameterName, parameter);
            }
        }

        var updatedInstance = new RuleDefinition.Instance(instanceName, parameterListBuilder.Build());
        return _ruleValidator.Validate(updatedInstance) ? updatedInstance : null;
    }
}
