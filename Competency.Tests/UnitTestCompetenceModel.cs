using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Competency.Models;
using Xunit;
namespace Competency.Tests;

public class UnitCompetenceModel
{
    [Fact]
    public void CanCreateCompetence()
    {
        // Arrange
        var competence = new Competence
        {
            Nom = "Programming",
            Libelle = "Ability to write code",
            Niveau = Competence.Grade.Intermédiaire
        };
        // Act & Assert
        Assert.NotNull(competence);
        Assert.Equal("Programming", competence.Nom);
        Assert.Equal("Ability to write code", competence.Libelle);
        Assert.Equal(Competence.Grade.Intermédiaire, competence.Niveau);
    }

    [Fact]
    public void CanGetGuid()
    {
        // Arrange
        var competence = new Competence();
        var guid = competence.ID;

        // Act & Assert
        Assert.Equal(Guid.Empty, guid);
    }
    [Fact]
    public void NomIsRequired()
    {
        // Arrange
        var competence = new Competence
        {
            Libelle = "Ability to write code",
            Niveau = Competence.Grade.Intermédiaire
        };

        // Act
        var validationContext = new ValidationContext(competence);
        var validationResults = new List<ValidationResult>();

        // Assert
        Assert.False(Validator.TryValidateObject(competence, validationContext, validationResults, true));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Nom"));
    }
    [Fact]
    public void LibelleIsRequired()
    {
        // Arrange
        var competence = new Competence
        {
            Nom = "Programming",
            Niveau = Competence.Grade.Intermédiaire
        };

        // Act
        var validationContext = new ValidationContext(competence);
        var validationResults = new List<ValidationResult>();

        // Assert
        Assert.False(Validator.TryValidateObject(competence, validationContext, validationResults, true));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Libelle"));
    }
    [Fact]
    public void NiveauIsRequired()
    {
        // Arrange
        var competence = new Competence
        {
            Nom = "Programming",
            Libelle = "Ability to write code"
        };

        // Act
        var validationContext = new ValidationContext(competence);
        var validationResults = new List<ValidationResult>();

        // Assert
        Assert.False(Validator.TryValidateObject(competence, validationContext, validationResults, true));
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Niveau"));
    }
}