using Microsoft.EntityFrameworkCore;
using Competency.Models;
namespace Competency.DataBaseContext;

public class CompetenciesMigrationContext : DbContext
{
    public CompetenciesMigrationContext(DbContextOptions<CompetenciesMigrationContext> options)
        : base(options)
    {
    }
    public DbSet<Competence> Competences { get; set; } = null!;
    public DbSet<CompetenceEmploye> CompetenceEmployes { get; set; } = null!;
    public DbSet<CompetenceFormation> CompetenceFormations { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompetenceEmploye>()
        .HasKey(c => c.ID);
        modelBuilder.Entity<CompetenceEmploye>()
        .Property(ce => ce.CompetenceId)
        .IsRequired();
        modelBuilder.Entity<CompetenceEmploye>()
        .Property(ce => ce.EmployeId)
        .IsRequired();
        modelBuilder.Entity<CompetenceFormation>()
       .HasKey(cf => cf.ID);
        modelBuilder.Entity<CompetenceFormation>()
      .Property(cf => cf.FormationId)
      .IsRequired();
        modelBuilder.Entity<CompetenceFormation>()
      .Property(cf => cf.CompetenceId)
      .IsRequired();
    }
}
