using Competency.DataBaseContext;
using Competency.Models;
namespace Competency.Repositories;
public class CompetenceRepository
{
    private readonly CompetenciesMigrationContext dbContext;
    public CompetenceRepository(CompetenciesMigrationContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public IQueryable<Competence> GetCompetencies()
    {
        return dbContext.Competences;
    }
    public async Task<Competence> CreateCompetency(Competence competence)
    {
        await dbContext.Competences.AddAsync(competence);
        await dbContext.SaveChangesAsync();
        return competence;
    }
    public async Task SetCompetency()
    {
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteGetCompetency()
    {
        await dbContext.SaveChangesAsync();
    }
}