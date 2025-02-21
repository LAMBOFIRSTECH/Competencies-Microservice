using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Competency.Models;
using Xunit;
namespace Competency.Tests;
public class UnitTestCompetenceEmployeModel
{
    [Fact]
    public void CompetenceEmploye_Should_Be_Valid_With_Required_Fields()
    {
        // Arrange
        var competenceEmploye = new CompetenceEmploye
        {
            ID = Guid.NewGuid(),
            EmployeId = Guid.NewGuid(),
            Niveau = CompetenceEmploye.Grade.Intermédiaire
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(competenceEmploye, null, null);
        var isValid = Validator.TryValidateObject(competenceEmploye, validationContext, validationResults, true);
        Assert.True(isValid);
    }
}