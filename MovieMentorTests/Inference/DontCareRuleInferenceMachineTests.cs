using System.Collections.Generic;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;
using Xunit;

namespace MovieMentorTests.Inference;

public class DontCareRuleInferenceMachineTests
{
    private readonly InferenceMachine _sut;

    public DontCareRuleInferenceMachineTests()
    {
        _sut = new InferenceMachine(new Dictionary<string, IList<RuleDefinition>>
        {
            {
                "Category", new List<RuleDefinition>
                {
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
                        .AddParameter("Name", new Parameter.SingleValue("Value1"))
                        .AddParameter("Type", new Parameter.SingleValue("One"))
                        .Build()),
                    new RuleDefinition.Instance("Category", new ParameterList.Builder()
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
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRule()
    {
        // Temp(?0) = Category(_, ?0), Number(?0) 
        // Temp(?0) = [[One], [Two]]
        var ruleDefinition = new RuleDefinition.Composite(new RuleDefinition.Instance("Temp", new ParameterList.Builder()
                .AddParameter("Result", new Parameter.Reference(0))
                .Build()),
            new List<RuleDefinition.Instance>
            {
                new("Category", new ParameterList.Builder()
                    .AddParameter("Name", new Parameter.DontCare())
                    .AddParameter("Type", new Parameter.Reference(0))
                    .Build()),
                new("Number", new ParameterList.Builder()
                    .AddParameter("Value", new Parameter.Reference(0))
                    .Build()),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleDefinition.Instance("Temp", new ParameterList.Builder()
            .AddParameter("Result", new Parameter.Reference(0))
            .Build());

        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(2, possibilities.Count);
        foreach (var parameterList in possibilities)
        {
            Assert.Single(parameterList);
        }

        Assert.Equal("One", (possibilities[0]["Result"] as Parameter.SingleValue)!.Value);
        Assert.Equal("Two", (possibilities[1]["Result"] as Parameter.SingleValue)!.Value);
    }
}
