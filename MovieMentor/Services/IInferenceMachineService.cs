using MovieMentorCore.Models;

namespace MovieMentor.Services;

public interface IInferenceMachineService
{
    void Init();

    IList<string[]> Infer(RuleInstance ruleInstance);
}
