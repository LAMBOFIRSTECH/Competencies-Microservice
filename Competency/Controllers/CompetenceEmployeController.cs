using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Competency.Services;
using Competency.Commands;
namespace Competency.Controllers;
[ApiController]
public class CompetenceEmployeController : ControllerBase
{
    private readonly CompetenceEmployeService competenceEmployeService;
    public CompetenceEmployeController(CompetenceEmployeService competenceEmployeService)
    {
        this.competenceEmployeService = competenceEmployeService;
    }
    /// <summary>
    /// Affiche la liste de toutes les compétences.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("competency-id/{id}/employe-id/{employeId}")]
    public async Task<IActionResult> AddCompetency([FromBody] AddCompetenceToEmployeCommand command)
    {
        return Ok();
    }
}