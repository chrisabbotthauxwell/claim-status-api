using ClaimStatusAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
public class ClaimsServiceTests
{
    private readonly ClaimsService _claimsService;
    private readonly Mock<ILogger<ClaimsService>> _loggerMock;
    public ClaimsServiceTests()
    {
        _loggerMock = new Mock<ILogger<ClaimsService>>();
        _claimsService = new ClaimsService(_loggerMock.Object);
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
}