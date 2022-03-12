using MovieMentorCore.Models;

namespace MovieMentor.Services;

public class KnowledgeBaseLoader : IKnowledgeBaseLoader
{
    public IDictionary<string, IList<RuleDefinition>> GetRules()
    {
        return new Dictionary<string, IList<RuleDefinition>>();
    }
}
