using Microsoft.AspNetCore.Mvc;
using MovieMentor.DTO;
using MovieMentor.Services;
using MovieMentorCore.Models;

namespace MovieMentor.Controllers;

public static class InferenceController
{
    public static IEnumerable<string> Infer([FromBody] RuleInstanceDto ruleInstanceDto,
        [FromServices] InferenceMachineService inferenceMachineService)
    {
        var ruleInstance = Convert(ruleInstanceDto);

        var possibilities = inferenceMachineService.Infer(ruleInstance);

        return possibilities.Select(p => p.FirstOrDefault() ?? "");
    }

    private static RuleInstance Convert(RuleInstanceDto ruleInstanceDto)
    {
        return new RuleInstance(ruleInstanceDto.Name, new List<Parameter>());
    }
}
