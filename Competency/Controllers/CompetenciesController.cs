using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Competency.Interfaces;
using Competency.Models;
using Competency.Services;
namespace Competency.Controllers;
[ApiController]
public class CompetenciesController : ControllerBase
{
    private readonly ICompetenceService competenceService;
    public CompetenciesController(ICompetenceService competenceService)
    {
        this.competenceService = competenceService;
    }
    /// <summary>
    /// Affiche la liste de toutes les compétences.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("competencies")]
    public async Task<IActionResult> GetCompetencies()
    {
        var competences = await competenceService.GetCompetencies();
        if (!competences.Any())
            return NoContent();
        return CreatedAtAction(nameof(GetCompetencies), new { competences });
    }
    /// <summary>
    /// Affiche une compétence spécifique.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("competency/{id}")]
    public async Task<IActionResult> GetCompetency(Guid id)
    {
        await Task.Delay(20);
        return NoContent();
    }
    /// <summary>
    /// Crée une compétence dans le système.
    /// </summary>
    /// <remarks>
    /// <para>Sample request:</para>
    /// <para>
    ///     POST /competency
    ///     {
    ///        "Nom": "CI/CD",
    ///        "Libelle": "Système d'information"
    ///        "Niveau": enum { Debutant, Intermédiaire, Senior, Expert },
    ///        "Active": true
    ///     }
    /// </para>
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("competency")]
    public async Task<IActionResult> CreateCompetency([FromBody] Competence competence)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var competenceResult = await competenceService.CreateCompetency(competence);
        return CreatedAtAction(nameof(CreateCompetency), new
        {
            result = new CompetenceResult()
            {
                Response = true,
                Competence = competenceResult,
                Message="Competency has been successfull created"
            }
        });
    }
}