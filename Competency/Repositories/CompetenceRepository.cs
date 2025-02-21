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
    public async Task<Competence> CreateGetCompetency(Competence competence)
    {
        await dbContext.Competences.AddAsync(competence);
        await dbContext.SaveChangesAsync();
        return competence;
    }
    public async Task SetCompetency(Competence competence)
    {
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteGetCompetency(Guid id)
    {
        var competence = dbContext.Competences.Where(c => c.ID == id && c.Active.Equals(true)).FirstOrDefault();
        dbContext.Competences.Remove(competence!);
        await dbContext.SaveChangesAsync();
    }
}