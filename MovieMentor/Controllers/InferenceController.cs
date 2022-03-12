using Microsoft.AspNetCore.Mvc;
using MovieMentor.Models;
using MovieMentor.Services;
using MovieMentorCore.Inference;
using MovieMentorCore.Models;

namespace MovieMentor.Controllers;

[ApiController]
[Route("[controller]")]
public class InferenceController : ControllerBase
{
    private readonly IKnowledgeBaseLoader _knowledgeBaseLoader;

    public InferenceController(IKnowledgeBaseLoader knowledgeBaseLoader)
    {
        _knowledgeBaseLoader = knowledgeBaseLoader;
    }

    [HttpPost]
    public IEnumerable<string> Infer(RuleInstanceDto ruleInstanceDto)
    {
        var inferenceMachine = new InferenceMachine(_knowledgeBaseLoader.GetRules());

        var ruleInstance = Convert(ruleInstanceDto);

        var possibilities = inferenceMachine.Infer(ruleInstance);

        return possibilities.Select(p => p.FirstOrDefault() ?? "");
    }

    private RuleInstance Convert(RuleInstanceDto ruleInstanceDto)
    {
        return new RuleInstance(ruleInstanceDto.Name, new List<Parameter>());
    }
}
