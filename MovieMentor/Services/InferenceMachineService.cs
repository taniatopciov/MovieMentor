using MovieMentorCore.Inference;
using MovieMentorCore.Models;

namespace MovieMentor.Services;

public class InferenceMachineService : IInferenceMachineService
{
    private readonly IKnowledgeBaseLoader _knowledgeBaseLoader;
    private InferenceMachine? _inferenceMachine;

    public InferenceMachineService(IKnowledgeBaseLoader knowledgeBaseLoader)
    {
        _knowledgeBaseLoader = knowledgeBaseLoader;
    }
    
    public void Init()
    {
        _inferenceMachine = new InferenceMachine(_knowledgeBaseLoader.GetRules());
    }

    public IList<string[]> Infer(RuleInstance ruleInstance)
    {
        if (_inferenceMachine == null)
        {
            return new List<string[]>();
        }
        
        return _inferenceMachine.Infer(ruleInstance);
    }
}
