using MediatR;
using Competency.Models;
namespace Competency.Commands;
public class AddCompetenceToEmployeCommand : IRequest
{
    public Guid EmployeId { get; set; }
    public Guid CompetenceId { get; set; }
    public CompetenceEmploye.Grade Niveau { get; set; }
}
