using System.ComponentModel.DataAnnotations;
namespace Competency.Models;
/// <summary>
/// Représente une compétence dans le système.
/// </summary>
public class Competence
{
    [Key]
    public Guid ID { get; set; }

    [Required]
    [StringLength(100)]
    public string? Libelle { get; set; }
}