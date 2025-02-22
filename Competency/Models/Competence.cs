using System.ComponentModel.DataAnnotations;
namespace Competency.Models;
/// <summary>
/// Représente une compétence dans le système.
/// </summary>
public class Competence
{
    [Key]
    public Guid ID { get; set;}
    [Required]
    [StringLength(100)]
    public string? Nom { get; set; }
    [Required]
    [StringLength(100)]
    public string? Libelle { get; set; }
    public enum NiveauCompetence  { Notions, Intermédiaire, Avancé, Expert }
    [Required]
    public NiveauCompetence ? Niveau { get; set; }
    public bool? Active  { get; set; }
}