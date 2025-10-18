using System.Text.Json;
using ClaimStatusAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using OpenAI.Chat;
using ClaimStatusAPI.Models;

public class ClaimsServiceTests
{
    private readonly ClaimsService _claimsService;
    private readonly Mock<ILogger<ClaimsService>> _loggerMock;
    private readonly Mock<IOpenAiService> _openAiServiceMock;
    public ClaimsServiceTests()
    {
        _loggerMock = new Mock<ILogger<ClaimsService>>();
        _openAiServiceMock = new Mock<IOpenAiService>();
        _claimsService = new ClaimsService(_loggerMock.Object, _openAiServiceMock.Object);
    }

    [Fact]
    public void GetClaimById_ShouldReturnClaim_WhenClaimExists()
    {
        // Arrange
        var existingClaimId = "1001";

        // Act
        var claim = _claimsService.GetById(existingClaimId);

        // Assert
        Assert.NotNull(claim);
        Assert.Equal(existingClaimId, claim.Id);
    }

    [Fact]
    public void GetClaimById_ShouldReturnNull_WhenClaimDoesNotExist()
    {
        // Arrange
        var nonExistingClaimId = "9999";

        // Act
        var claim = _claimsService.GetById(nonExistingClaimId);

        // Assert
        Assert.Null(claim);
    }

    [Fact]
    public void GetSummaryForClaimById_ShouldReturnNull_WhenClaimDoesNotExist()
    {
        // Arrange
        var nonExistingClaimId = "9999";

        // Act
        var summaryTask = _claimsService.GetSummaryByIdAsync(nonExistingClaimId);
        summaryTask.Wait();
        var summary = summaryTask.Result;

        // Assert
        Assert.Null(summary);
    }
}