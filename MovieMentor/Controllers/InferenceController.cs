using Microsoft.AspNetCore.Mvc;
using MovieMentor.Models;
using MovieMentor.Services;
using MovieMentorCore.Models;

namespace MovieMentor.Controllers;

[ApiController]
[Route("[controller]")]
public class InferenceController : ControllerBase
{
    private readonly InferenceMachineService _inferenceMachineService;

    public InferenceController(InferenceMachineService inferenceMachineService)
    {
        _inferenceMachineService = inferenceMachineService;
    }

    [HttpPost]
    public IEnumerable<string> Infer(RuleInstanceDto ruleInstanceDto)
    {
        var ruleInstance = Convert(ruleInstanceDto);

        var possibilities = _inferenceMachineService.Infer(ruleInstance);

        return possibilities.Select(p => p.FirstOrDefault() ?? "");
    }

    private static RuleInstance Convert(RuleInstanceDto ruleInstanceDto)
    {
        return new RuleInstance(ruleInstanceDto.Name, new List<Parameter>());
    }
}
