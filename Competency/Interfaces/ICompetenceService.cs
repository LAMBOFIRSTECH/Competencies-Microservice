using Competency.Models;

namespace Competency.Interfaces;
public interface ICompetenceService
{
    Task<ICollection<Competence>> GetCompetencies(Func<IQueryable<Competence>, IQueryable<Competence>>? filter = null);
    Task<Competence?> GetCompetency(Guid id);
    Task<Competence> CreateCompetency(Competence competence);
    Task<CompetenceResult> SetCompetency(Guid id);
    Task DeleteGetCompetency(Guid id);
}