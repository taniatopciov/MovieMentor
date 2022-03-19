using MovieMentor.DTO;
using MovieMentorCore.Models;

namespace MovieMentor.Services;

public interface IKnowledgeBaseLoader
{
    IEnumerable<ChoiceDto> GetChoices();

    IDictionary<string, IList<RuleDefinition>> GetRules();
}
