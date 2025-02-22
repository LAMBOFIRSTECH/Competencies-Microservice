using Competency.Models;

namespace Competency.Interfaces;
public interface ICompetenceEmployeService
{
    List<CompetenceEmploye> GetAllEmployeeCompetencies(Guid employeID);
}