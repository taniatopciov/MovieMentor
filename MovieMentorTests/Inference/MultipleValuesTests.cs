using System.Collections.Generic;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;
using Xunit;

namespace MovieMentorTests.Inference;

public class MultipleValuesTests
{
    private readonly InferenceMachine _sut;

    public MultipleValuesTests()
    {
        _sut = new InferenceMachine(new Dictionary<string, IList<RuleDefinition>>
        {
            {
                "Values", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Values", new ParameterList.Builder()
                        .AddParameter("Data", new Parameter.MultipleValues(new HashSet<string>
                        {
                            "Value1",
                            "Value2",
                        }))
                        .Build()),
                }
            },
        });
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenConcreteRuleAsInputRuleWhereTheArgumentDoesNotExistInRules()
    {
        // Values(Invalid_Arg: Value1|Value2) => [[]] 
        var ruleInstance = new RuleDefinition.Instance("Values", new ParameterList.Builder()
            .AddParameter("InvalidArg", new Parameter.MultipleValues(new HashSet<string>
            {
                "Value1",
                "Value2",
            }))
            .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenConcreteRuleAsInputRuleWhereTheArgumentIsNotOfTheCorrectType()
    {
        // Values(Data: Invalid_Arg) => [[]] 
        var ruleInstance = new RuleDefinition.Instance("Values", new ParameterList.Builder()
            .AddParameter("Data", new Parameter.SingleValue("Invalid_Arg"))
            .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenConcreteRuleAsInputRuleWhereTheArgumentHasNotTheCorrectValues()
    {
        // Values(Data: V5|V8) => [[]] 
        var ruleInstance = new RuleDefinition.Instance("Values", new ParameterList.Builder()
            .AddParameter("Data", new Parameter.MultipleValues(new HashSet<string>
            {
                "Value5",
                "Value8",
            }))
            .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }

    [Fact]
    public void
        Infer_ShouldReturnAllElements_GivenConcreteRuleAsInputRuleWhereTheArgumentHasAtLeastOneValueThatMatches()
    {
        // Values(Data: V5|V8) => [[Data: V5|V8]] 
        var ruleInstance = new RuleDefinition.Instance("Values", new ParameterList.Builder()
            .AddParameter("Data", new Parameter.MultipleValues(new HashSet<string>
            {
                "Nonexistent",
                "Value2",
            }))
            .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Single(possibilities[0]);
        var dataParameter = (possibilities[0]["Data"] as Parameter.MultipleValues)!;
        Assert.Equal(2, dataParameter.Values.Count);
        Assert.Contains("Nonexistent", dataParameter.Values);
        Assert.Contains("Value2", dataParameter.Values);
    }
    
    [Fact]
    public void
        Infer_ShouldReturnAllElements_GivenConcreteRuleAsInputRuleWhereTheArgumentIsReferenceParameter()
    {
        // Values(?0) => [[Data: V5|V8]] 
        var ruleInstance = new RuleDefinition.Instance("Values", new ParameterList.Builder()
            .AddParameter("Data", new Parameter.Reference(0))
            .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Single(possibilities[0]);
        var dataParameter = (possibilities[0]["Data"] as Parameter.MultipleValues)!;
        Assert.Equal(2, dataParameter.Values.Count);
        Assert.Contains("Value1", dataParameter.Values);
        Assert.Contains("Value2", dataParameter.Values);
    }
}
