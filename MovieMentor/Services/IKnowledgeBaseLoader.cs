using MovieMentorCore.Models;

namespace MovieMentor.Services;

public interface IKnowledgeBaseLoader
{
    IDictionary<string, IList<RuleDefinition>> GetRules();
}
