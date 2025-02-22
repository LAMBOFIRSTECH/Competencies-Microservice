using System;
using Competency.Models;
using Xunit;
namespace Competency.Tests;

public class UnitTestCompetenceFormationModel
{
    [Fact]
    public void CompetenceFormation_ShouldHaveValidProperties()
    {
        // Arrange
        var competenceFormation = new CompetenceFormation
        {
            ID = Guid.NewGuid(),
            FormationId = Guid.NewGuid(),
            CompetenceId = Guid.NewGuid()
        };

        // Act & Assert
        Assert.NotEqual(Guid.Empty, competenceFormation.ID);
        Assert.NotEqual(Guid.Empty, competenceFormation.FormationId);
        Assert.NotEqual(Guid.Empty, competenceFormation.CompetenceId);
    }
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
}
