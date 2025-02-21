using System.ComponentModel.DataAnnotations;
namespace Competency.Models;
/// <summary>
/// Représente une compétence dans le système.
/// </summary>
public class Competence
{
    [Key]
    private Guid ID { get; }
    [Required]
    [StringLength(100)]
    public string? Nom { get; set; }
    [Required]
    [StringLength(100)]
    public string? Libelle { get; set; }
    public enum Grade { Debutant, Intermédiaire, Senior, Expert }
    [Required]
    public Grade? Niveau { get; set; }

    public Guid GetGuid()
    {
        return ID;
    }
}