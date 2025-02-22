using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Competency.Interfaces;
using Competency.Models;
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
    /// Affiche une compétence spécifique en fonction de l'identifiant".
    /// </summary>
    /// <param name="id"></param>
    [AllowAnonymous]
    [HttpGet("competency-id/{id}")]
    public async Task<IActionResult> GetCompetency(Guid id)
    {
        var competence = await competenceService.GetCompetency(id);
        if (competence is null)
            return NotFound();
        return CreatedAtAction(nameof(GetCompetency), new { competence });
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
    ///        "Niveau": enum { Notions, Intermédiaire, Avancé, Expert },
    ///        "Active": true
    ///     }
    /// </para>
    /// </remarks>
    [AllowAnonymous]
    // [Authorize(Policy = "AdminPolicy")]
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
                Message = "Competency has been successfull created"
            }
        });
    }
    /// <summary>
    /// Met à jour une compétence en fonction de l'identifiant.
    /// </summary>
    /// <param name="id"></param>
    [AllowAnonymous]
    [HttpPut("competency-id/{id}")]
    public async Task<IActionResult> SetCompetency(Guid id)
    {
        var competenceResult = await competenceService.SetCompetency(id);
        return CreatedAtAction(nameof(SetCompetency), new
        {
            result = new CompetenceResult()
            {
                Response = competenceResult.Response,
                Competence = competenceResult.Competence,
                Message = competenceResult.Message
            }
        });
    }
    /// <summary>
    /// Désactive une compétence en fonction de l'identifiant".
    /// </summary>
    /// <param name="id"></param>
    [AllowAnonymous]
    [HttpDelete("competency-id/{id}")]
    public async Task<IActionResult> DeleteCompetency(Guid id)
    {
        var competenceResult = await competenceService.DeleteGetCompetency(id);
        return CreatedAtAction(nameof(DeleteCompetency), new
        {
            result = new CompetenceResult()
            {
                Response = competenceResult.Response,
                Competence = competenceResult.Competence,
                Message = competenceResult.Message
            }
        });
    }
}