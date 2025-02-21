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
        modelBuilder.Entity<Competence>()
		.HasKey(c=> c.ID);
    }
    // 	modelBuilder.Entity<Utilisateur>()
    // 	   .HasMany(u => u.LesTaches)
    // 	   .WithOne(t => t.Utilisateur)
    // 	   .HasForeignKey(t => t.UserId)
    // 	   .IsRequired()
    // 	   .OnDelete(DeleteBehavior.Cascade);
    // 	modelBuilder.Entity<Tache>();
    // 	modelBuilder.Entity<Projet>()
    // 	   .HasIndex(p => p.Code)
    // 	   .IsUnique(); // Montre que c'est une clé candidate
    // 	base.OnModelCreating(modelBuilder);
    // 	modelBuilder.Entity<Employe>()
    // 	   .HasIndex(e => e.Matricule)
    // 	   .IsUnique(); // Montre que c'est une clé candidate
    // 	base.OnModelCreating(modelBuilder);
    // }
}
