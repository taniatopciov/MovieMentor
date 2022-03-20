using System.Collections.Generic;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;
using Xunit;

namespace MovieMentorTests.Inference;

public class PartialRuleInferenceMachineTests
{
    private readonly InferenceMachine _sut;

    public PartialRuleInferenceMachineTests()
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
                    new RuleDefinition.Instance("Category",
                        new ParameterList.Builder()
                            .AddParameter("Name", new Parameter.SingleValue("Value3"))
                            .Build()),
                }
            },
            {
                "Tag", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("true"))
                        .Build()),
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("Value1"))
                        .Build()),
                }
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRule()
    {
        // Temp(?0) = Category(?0), Tag(?0) 
        // Temp(?0) = [[Value1]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build(),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build()),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance =
            new RuleDefinition.Instance("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Single(possibilities[0].Parameters);
        Assert.Equal("Value1", (possibilities[0]["Name"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithConcreteCategory()
    {
        // Temp(?0) = Category(Value2), Tag(?0) 
        // (?0) = Temp(?0) = [[true], [Value1]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build(),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.SingleValue("Value2"))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build()),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance =
            new RuleDefinition.Instance("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(2, possibilities.Count);
        foreach (var parameterList in possibilities)
        {
            Assert.Single(parameterList.Parameters);
        }

        Assert.Equal("true", (possibilities[0]["Name"] as Parameter.SingleValue)!.Value);
        Assert.Equal("Value1", (possibilities[1]["Name"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithConcreteTag()
    {
        // Temp(?0) = Category(?0), Tag(true) 
        // Temp(?0) = [[Value1], [Value2], [Value3]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build(),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.Reference(0))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.SingleValue("true"))
                    .Build()),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance =
            new RuleDefinition.Instance("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(3, possibilities.Count);
        foreach (var parameterList in possibilities)
        {
            Assert.Single(parameterList.Parameters);
        }

        Assert.Equal("Value1", (possibilities[0]["Name"] as Parameter.SingleValue)!.Value);
        Assert.Equal("Value2", (possibilities[1]["Name"] as Parameter.SingleValue)!.Value);
        Assert.Equal("Value3", (possibilities[2]["Name"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenPartialRuleWithAllConcreteRules()
    {
        // Temp(?0) = Category(Value2), Tag(Value1) 
        // (?0) = Temp(?0) = []
        var ruleDefinition = new RuleDefinition.Composite("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build(),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.SingleValue("Value2"))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.SingleValue("Value1"))
                    .Build()),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance =
            new RuleDefinition.Instance("Temp", new ParameterList.Builder()
                .AddParameter("Name", new Parameter.Reference(0))
                .Build());
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }
}
