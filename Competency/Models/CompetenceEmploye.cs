using System.ComponentModel.DataAnnotations;
namespace Competency.Models;
/// <summary>
/// Représente une compétence pour un employé dans le système.
/// </summary>
public class CompetenceEmploye
{
    [Key]
    public Guid ID { get; set; }
    [Required]
    public Guid EmployeId { get; set; }
    public Guid CompetenceId { get; set; }
    public enum Grade { Debutant, Intermédiaire, Senior, Expert }
    [Required]
    public Grade? Niveau { get; set; }
}