using Competency.Commands;
using Competency.Repositories;
using MediatR;
namespace Competency.Services;
public class CompetenceFormationService
{
    private readonly CompetenceFormationRepository competenceFormationRepository;
    private readonly ILogger<CompetenceFormationService> log;
     private readonly IMediator mediator;
    public CompetenceFormationService(ILogger<CompetenceFormationService> log, CompetenceFormationRepository competenceFormationRepository, IMediator mediator)
    {
        this.log = log;
        this.competenceFormationRepository = competenceFormationRepository;
        this.mediator = mediator;
    }
    public async Task AddCompetencyToFormationAsync(AddCompetenceToEmployeCommand competenceEmploye)
    {
        await competenceFormationRepository.AddCompetencyAsync(competenceEmploye);
    }
}
