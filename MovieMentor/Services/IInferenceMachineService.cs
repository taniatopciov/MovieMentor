using MovieMentorCore.Models;

namespace MovieMentor.Services;

public interface IInferenceMachineService
{
    IEnumerable<ParameterList> Infer(RuleDefinition.Instance ruleInstance);
}
