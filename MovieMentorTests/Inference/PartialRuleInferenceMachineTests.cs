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
                    new RuleDefinition.Concrete("Category", new List<Parameter.Concrete> { new("Value1") }),
                    new RuleDefinition.Concrete("Category", new List<Parameter.Concrete> { new("Value2") }),
                    new RuleDefinition.Concrete("Category", new List<Parameter.Concrete> { new("Value3") }),
                }
            },
            {
                "Tag", new List<RuleDefinition>
                {
                    new RuleDefinition.Concrete("Tag", new List<Parameter.Concrete> { new("true") }),
                    new RuleDefinition.Concrete("Tag", new List<Parameter.Concrete> { new("Value1") }),
                }
            }
        });
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRule()
    {
        // Temp(?0) = Category(?0), Tag(?0) 
        // Temp(?0) = [[Value1]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new List<Parameter> { new Parameter.Reference(0) },
            new List<RuleInstance>
            {
                new("Category", new List<Parameter> { new Parameter.Reference(0) }),
                new("Tag", new List<Parameter> { new Parameter.Reference(0) }),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleInstance("Temp", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Single(possibilities);
        Assert.Single(possibilities[0]);
        Assert.Equal("Value1", possibilities[0][0]);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithConcreteCategory()
    {
        // Temp(?0) = Category(Value2), Tag(?0) 
        // (?0) = Temp(?0) = [[true], [Value1]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new List<Parameter> { new Parameter.Reference(0) },
            new List<RuleInstance>
            {
                new("Category", new List<Parameter> { new Parameter.Concrete("Value2") }),
                new("Tag", new List<Parameter> { new Parameter.Reference(0) }),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleInstance("Temp", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(2, possibilities.Count);
        foreach (var possibility in possibilities)
        {
            Assert.Single(possibility);
        }

        Assert.Equal("true", possibilities[0][0]);
        Assert.Equal("Value1", possibilities[1][0]);
    }

    [Fact]
    public void Infer_ShouldReturnAllConcreteParameter_GivenPartialRuleWithConcreteTag()
    {
        // Temp(?0) = Category(?0), Tag(true) 
        // (?0) = Temp(?0) = [[Value1], [Value2], [Value3]]
        var ruleDefinition = new RuleDefinition.Composite("Temp", new List<Parameter> { new Parameter.Reference(0) },
            new List<RuleInstance>
            {
                new("Category", new List<Parameter> { new Parameter.Reference(0) }),
                new("Tag", new List<Parameter> { new Parameter.Concrete("true") }),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleInstance("Temp", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Equal(3, possibilities.Count);
        foreach (var possibility in possibilities)
        {
            Assert.Single(possibility);
        }

        Assert.Equal("Value1", possibilities[0][0]);
        Assert.Equal("Value2", possibilities[1][0]);
        Assert.Equal("Value3", possibilities[2][0]);
    }

    [Fact]
    public void Infer_ShouldReturnEmptyList_GivenPartialRuleWithAllConcreteRules()
    {
        // Temp(?0) = Category(Value2), Tag(Value1) 
        // (?0) = Temp(?0) = []
        var ruleDefinition = new RuleDefinition.Composite("Temp", new List<Parameter> { new Parameter.Reference(0) },
            new List<RuleInstance>
            {
                new("Category", new List<Parameter> { new Parameter.Concrete("Value2") }),
                new("Tag", new List<Parameter> { new Parameter.Concrete("Value1") }),
            });
        _sut.AddRuleDefinition("Temp", ruleDefinition);

        var ruleInstance = new RuleInstance("Temp", new List<Parameter> { new Parameter.Reference(0) });
        var possibilities = _sut.Infer(ruleInstance);

        Assert.Empty(possibilities);
    }
}
