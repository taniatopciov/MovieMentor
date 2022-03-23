using System.Collections.Generic;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;
using Xunit;

namespace MovieMentorTests.Inference;

public class PartialRulesWithInnerParametersTests
{
    private readonly InferenceMachine _sut;

    public PartialRulesWithInnerParametersTests()
    {
        // Category(1, Value1, One)
        // Category(2, Value2, Two)
        // Number(One)
        // Number(Two)
        // Number(Three)
        // Tag(Value1)
        // Tag(Value4)
        // Tag(Value7)
        _sut = new InferenceMachine(new Dictionary<string, IList<RuleDefinition>>
        {
            {
                "Category", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
                        .AddParameter("ID", new Parameter.SingleValue("1"))
                        .AddParameter("Name", new Parameter.SingleValue("Value1"))
                        .AddParameter("Type", new Parameter.SingleValue("One"))
                        .Build()),
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
                        .AddParameter("ID", new Parameter.SingleValue("2"))
                        .AddParameter("Name", new Parameter.SingleValue("Value2"))
                        .AddParameter("Type", new Parameter.SingleValue("Two"))
                        .Build()),
                }
            },
            {
                "Number", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Number", new ParameterList.Builder()
                        .AddParameter("Value", new Parameter.SingleValue("One"))
                        .Build()),
                    new RuleDefinition.Instance("Number", new ParameterList.Builder()
                        .AddParameter("Value", new Parameter.SingleValue("Two"))
                        .Build()),
                    new RuleDefinition.Instance("Number", new ParameterList.Builder()
                        .AddParameter("Value", new Parameter.SingleValue("Three"))
                        .Build()),
                }
            },
            {
                "Tag", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("T", new Parameter.SingleValue("Value1"))
                        .Build()),
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("T", new Parameter.SingleValue("Value4"))
                        .Build()),
                    new RuleDefinition.Instance("Tag", new ParameterList.Builder()
                        .AddParameter("T", new Parameter.SingleValue("Value7"))
                        .Build()),
                }
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithInnerVariables()
    {
        // Temp(?0) = Category(?0, ?1, ?2), Number(?2), Tag(?1) 
        // Temp(?0) = [[1]]
        var ruleDefinition = new RuleDefinition.Composite(new RuleDefinition.Instance("Temp",
                new ParameterList.Builder()
                    .AddParameter("Result", new Parameter.Reference(0))
                    .Build()),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("ID", new Parameter.Reference(0))
                    .AddParameter("Name", new Parameter.Reference(1))
                    .AddParameter("Type", new Parameter.Reference(2))
                    .Build()),
                new("Number", new ParameterList.Builder()
                    .AddParameter("Value", new Parameter.Reference(2))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("T", new Parameter.Reference(1))
                    .Build())
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleDefinition.Instance("Temp", new ParameterList.Builder()
            .AddParameter("Result", new Parameter.Reference(0))
            .Build());

        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Single(possibilities[0]);
        Assert.Equal("1", (possibilities[0]["Result"] as Parameter.SingleValue)!.Value);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithInnerVariables_OnlyOnePossibility()
    {
        // Temp(?0, true) = Category(?0, ?1, One), Tag(?1) 
        // Temp(?0, true) = [[1, true]]
        var ruleDefinition = new RuleDefinition.Composite(new RuleDefinition.Instance("Temp",
                new ParameterList.Builder()
                    .AddParameter("Result", new Parameter.Reference(0))
                    .AddParameter("Result2", new Parameter.SingleValue("true"))
                    .Build()),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("ID", new Parameter.Reference(0))
                    .AddParameter("Name", new Parameter.Reference(1))
                    .AddParameter("Type", new Parameter.SingleValue("One"))
                    .Build()),
                new("Tag", new ParameterList.Builder()
                    .AddParameter("T", new Parameter.Reference(1))
                    .Build())
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleDefinition.Instance("Temp", new ParameterList.Builder()
            .AddParameter("Result", new Parameter.Reference(0))
            .AddParameter("Result2", new Parameter.SingleValue("true"))
            .Build());

        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Equal(2, possibilities[0].Count);
        Assert.Equal("1", (possibilities[0]["Result"] as Parameter.SingleValue)!.Value);
        Assert.Equal("true", (possibilities[0]["Result2"] as Parameter.SingleValue)!.Value);
    }
}
