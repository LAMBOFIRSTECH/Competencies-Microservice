using Competency.Commands;
using Competency.DataBaseContext;
using Competency.Models;
namespace Competency.Repositories;
public class CompetenceFormationRepository
{
    private readonly CompetenciesMigrationContext dbContext;
    public CompetenceFormationRepository(CompetenciesMigrationContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task AddCompetencyAsync(AddCompetenceToEmployeCommand competenceEmploye)
    {
        await dbContext.SaveChangesAsync();
    }
}
