using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Competency.Interfaces;
using Competency.Models;
namespace Competency.Controllers;
[ApiController]
public class CompetenciesController : ControllerBase
{

    public CompetenciesController()
    {

    }
    /// <summary>
    /// Affiche la liste de toutes les compétences.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("competencies")]
    public async Task<IActionResult> GetCompetencies()
    {
        await Task.Delay(20);
        return NoContent();
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
}