using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Competency.Models;
using Xunit;
namespace Competency.Tests;

public class UnitTestCompetenceFormationModel
{
    [Fact]
    public void CompetenceFormation_ShouldHaveValidID()
    {
        // Arrange
        var competenceFormation = new CompetenceFormation
        {
            // Act
            ID = Guid.NewGuid()
        };

        // Assert
        Assert.NotEqual(Guid.Empty, competenceFormation.ID);
    }
    [Fact]
    public void CompetenceFormation_ShouldRequireCompetence()
    {
        // Arrange
        var competenceFormation = new CompetenceFormation
        {
            // Act
            competence = null
        };

        // Assert
        var validationContext = new ValidationContext(competenceFormation);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(competenceFormation, validationContext, validationResults, true);

        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CompetenceFormation.competence)));
    }
}
