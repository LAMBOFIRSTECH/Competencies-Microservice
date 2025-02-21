using Competency.Interfaces;
using Competency.Models;
using Competency.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Competency.Services;
public class CompetenceService : ICompetenceService
{
    private readonly IConfiguration configuration;
    private readonly CompetenceRepository competenceRepository;
    private readonly ILogger<CompetenceService> log;
    public CompetenceService(IConfiguration configuration, ILogger<CompetenceService> log, CompetenceRepository competenceRepository)
    {
        this.configuration = configuration;
        this.log = log;
        this.competenceRepository = competenceRepository;
    }
    public async Task<ICollection<Competence>> GetCompetencies(Func<IQueryable<Competence>, IQueryable<Competence>>? filter = null)
    {
        IQueryable<Competence> query = competenceRepository.GetCompetencies();
        if (filter != null)
        {
            query = filter(query);
        }
        return await query.ToListAsync();
    }
    public async Task<Competence?> GetCompetency(Guid id)
    {
        return (await GetCompetencies(query => query.Where(c => c.Active.Equals(true) && c.ID == id))).FirstOrDefault();
    }
    public async Task<Competence> CreateCompetency(Competence competence)
    {
        return await competenceRepository.CreateGetCompetency(competence);
    }
    public async Task<CompetenceResult> SetCompetency(Guid id)
    {
        var competence = (await GetCompetencies(query => query.Where(c => c.Active.Equals(true) && c.ID == id))).FirstOrDefault();
        if (competence is null)
            return new CompetenceResult()
            {
                Response = false,
                Competence = null,
                Message = "Not found or comptency status is not actiated"
            };
        
        await competenceRepository.SetCompetency(competence!);
        return new CompetenceResult() { Response = true, Competence = competence, Message="succesfull update competency" };
    }
    public async Task DeleteGetCompetency(Guid id)
    {
        await competenceRepository.DeleteGetCompetency(id);
    }
}