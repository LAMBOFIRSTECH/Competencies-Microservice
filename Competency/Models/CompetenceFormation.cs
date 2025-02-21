using System.ComponentModel.DataAnnotations;
namespace Competency.Models;
/// <summary>
/// Représente une compétence pour une formation dans le système.
/// </summary>
public class CompetenceFormation
{
    [Key]
    public Guid ID { get; set; }
    [Required]
    public Guid formationId { get; set; }
    [Required]
    public Competence? competence { get; set; }
}