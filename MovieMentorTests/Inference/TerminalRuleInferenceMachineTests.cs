using System.Collections.Generic;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;
using Xunit;

namespace MovieMentorTests.Inference;

public class TerminalRuleInferenceMachineTests
{
    private readonly InferenceMachine _sut;

    public TerminalRuleInferenceMachineTests()
    {
        _sut = new InferenceMachine(new Dictionary<string, IList<RuleDefinition>>
        {
            {
                "Category", new List<RuleDefinition>
                {
                    new RuleDefinition.Concrete("Category", new List<Parameter.Concrete> { new("Value1") }),
                    new RuleDefinition.Concrete("Category", new List<Parameter.Concrete> { new("Value2") }),
                }
            },
            {
                "Tag", new List<RuleDefinition>
                {
                    new RuleDefinition.Concrete("Tag", new List<Parameter.Concrete> { new("true") }),
                }
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenConcreteRuleAsInputRuleWhereTheArgumentDoesNotExistInRules()
    {
        // Category(Invalid_Arg) => [[]] 
        var ruleInstance = new RuleInstance("Category", new List<Parameter> { new Parameter.Concrete("Invalid_Arg") });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }

    [Fact]
    public void Infer_ShouldReturnConcreteParameter_GivenConcreteRuleAsInputRule()
    {
        // Category(Value1) => [[Value1]] 
        var ruleInstance = new RuleInstance("Category", new List<Parameter> { new Parameter.Concrete("Value1") });
        var possibilities = _sut.Infer(ruleInstance);

        var results = possibilities[0];

        Assert.Single(possibilities);
        Assert.Single(results);
        Assert.Equal("Value1", results[0]);
    }

    [Fact]
    public void Infer_ShouldReturnOneConcreteParameter_GivenRulesOnlyWithConcreteParameters()
    {
        // Tag(?0) => [[true]]
        var ruleInstance = new RuleInstance("Tag", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        var results = possibilities[0];

        Assert.Single(possibilities);
        Assert.Single(results);
        Assert.Equal("true", results[0]);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenRulesOnlyWithConcreteParameters()
    {
        // Category(?0) = [[Value1], [Value2]]
        var ruleInstance = new RuleInstance("Category", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(2, possibilities.Count);
        foreach (var possibility in possibilities)
        {
            Assert.Single(possibility);
        }

        Assert.Equal("Value1", possibilities[0][0]);
        Assert.Equal("Value2", possibilities[1][0]);
    }
}
