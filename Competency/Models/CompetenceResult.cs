namespace Competency.Models;
public class CompetenceResult
{
    public bool Response { get; set; }
    public Competence? Competence { get; set; }
    public string? Message { get; set; }
}