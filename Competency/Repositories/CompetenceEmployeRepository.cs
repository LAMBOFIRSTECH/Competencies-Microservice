using Competency.DataBaseContext;
using Competency.Models;
namespace Competency.Repositories;
public class CompetenceEmployeRepository
{
    private readonly CompetenciesMigrationContext dbContext;
    public CompetenceEmployeRepository(CompetenciesMigrationContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task AddCompetencyAsync(CompetenceEmploye competenceEmploye)
    {
        await dbContext.CompetenceEmployes.AddAsync(competenceEmploye);
        await dbContext.SaveChangesAsync();
    }
    public async Task DeleteAssignedEmployeeCompetencyAsync(Guid employeID)
    {
        var competenceEmploye = dbContext.CompetenceEmployes.Find(employeID);
        dbContext.CompetenceEmployes.Remove(competenceEmploye!);
        await dbContext.SaveChangesAsync();
    }
    public List<CompetenceEmploye> GetAllEmployeeCompetencies(Guid employeID)
    {
        return dbContext.CompetenceEmployes
        .Where(c => c.EmployeId == employeID)
        .ToList();
    }
}
