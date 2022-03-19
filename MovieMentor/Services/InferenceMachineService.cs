using MovieMentorCore.Inference;
using MovieMentorCore.Models;

namespace MovieMentor.Services;

public class InferenceMachineService : IInferenceMachineService
{
    private readonly IKnowledgeBaseLoader _knowledgeBaseLoader;

    public InferenceMachineService(IKnowledgeBaseLoader knowledgeBaseLoader)
    {
        _knowledgeBaseLoader = knowledgeBaseLoader;
    }

    public IEnumerable<string[]> Infer(RuleInstance ruleInstance)
    {
        var inferenceMachine = new InferenceMachine(_knowledgeBaseLoader.GetRules());

        return inferenceMachine.Infer(ruleInstance);
    }
}
