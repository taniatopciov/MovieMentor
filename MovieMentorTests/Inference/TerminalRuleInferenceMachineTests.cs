﻿using System.Collections.Generic;
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
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("Value1"))
                        .Build()),
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("Value2"))
                        .Build()),
                }
            },
            {
                "Tag", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("true"))
                        .Build()),
                }
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenConcreteRuleAsInputRuleWhereTheArgumentDoesNotExistInRules()
    {
        // Category(Invalid_Arg) => [[]] 
        var ruleInstance =
            new RuleDefinition.Instance("Category", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.SingleValue("Invalid_Arg"))
                .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }

    [Fact]
    public void Infer_ShouldReturnConcreteParameter_GivenConcreteRuleAsInputRule()
    {
        // Category(Value1) => [[Value1]] 
        var ruleInstance =
            new RuleDefinition.Instance("Category", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.SingleValue("Value1"))
                .Build());

        var possibilities = _sut.Infer(ruleInstance);

        var results = possibilities[0];

        Assert.Single(possibilities);
        Assert.Single(results.Parameters);
        Assert.Equal("Value1", (results["Name"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnOneConcreteParameter_GivenRulesOnlyWithConcreteParameters()
    {
        // Tag(?0) => [[true]]
        var ruleInstance =
            new RuleDefinition.Instance("Tag",
                new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build());
        var possibilities = _sut.Infer(ruleInstance);

        var results = possibilities[0];

        Assert.Single(possibilities);
        Assert.Single(results.Parameters);
        Assert.Equal("true", (results["Name"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenRulesOnlyWithConcreteParameters()
    {
        // Category(?0) = [[Value1], [Value2]]
        var ruleInstance =
            new RuleDefinition.Instance("Category",
                new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(2, possibilities.Count);
        foreach (var possibility in possibilities)
        {
            Assert.Single(possibility.Parameters);
        }

        Assert.Equal("Value1", (possibilities[0]["Name"] as Parameter.SingleValue)!.Value);
        Assert.Equal("Value2", (possibilities[1]["Name"] as Parameter.SingleValue)!.Value);
    }
}
