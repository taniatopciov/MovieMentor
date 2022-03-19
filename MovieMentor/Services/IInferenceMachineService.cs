using MovieMentorCore.Models;

namespace MovieMentor.Services;

public interface IInferenceMachineService
{
    IEnumerable<string[]> Infer(RuleInstance ruleInstance);
}
