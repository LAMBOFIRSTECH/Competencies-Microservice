using Competency.Models;
using Xunit;
namespace Competency.Tests;

public class UnitTestCompetenceResultModel
{
    [Fact]
    public void Result_Should_Set_Response_Correctly()
    {
        // Arrange
        var result = new CompetenceResult
        {
            // Act
            Response = true
        };

        // Assert
        Assert.True(result.Response);
    }
    [Fact]
    public void Result_Should_Set_Competence_Correctly()
    {
        // Arrange
        var competence = new Competence();
        var result = new CompetenceResult
        {
            // Act
            Competence = competence
        };

        // Assert
        Assert.Equal(competence, result.Competence);
    }
    [Fact]
    public void Result_Should_Allow_Null_Competence()
    {
        // Arrange
        var result = new CompetenceResult
        {
            // Act
            Competence = null
        };

        // Assert
        Assert.Null(result.Competence);
    }
}