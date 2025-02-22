using Competency.Interfaces;
using Competency.Models;
using Competency.Repositories;
namespace Competency.Services;
public class CompetenceEmployeService : ICompetenceEmployeService
{
    private readonly CompetenceEmployeRepository competenceEmployeRepository;
    private readonly ILogger<CompetenceEmployeService> log;
    public CompetenceEmployeService(ILogger<CompetenceEmployeService> log, CompetenceEmployeRepository competenceEmployeRepository)
    {
        this.log = log;
        this.competenceEmployeRepository = competenceEmployeRepository;
    }
    public async Task AddCompetencyToEmployeAsync(CompetenceEmploye competenceEmploye)
    {
        await competenceEmployeRepository.AddCompetencyAsync(competenceEmploye);
    }
    public async Task UpdateEmployeGradeOnCompetencyAsync(CompetenceEmploye competenceEmploye)
    {
        await competenceEmployeRepository.AddCompetencyAsync(competenceEmploye);
    }
    public void DeleteAssignedEmployeeCompetency(Guid employeID)
    {
        if (employeID == Guid.Empty)
        {
            throw new ArgumentException("Employe ID is empty");
        }
        GetAllEmployeeCompetencies(employeID).ForEach(async c => await competenceEmployeRepository.DeleteAssignedEmployeeCompetencyAsync(c.ID));
    }
    public List<CompetenceEmploye> GetAllEmployeeCompetencies(Guid employeID)
    {
        //cas ou l'id n'existe pas
        if (employeID == Guid.Empty)
        {
            throw new ArgumentException("Employe ID is empty");
        }
        return competenceEmployeRepository.GetAllEmployeeCompetencies(employeID);
    }
}