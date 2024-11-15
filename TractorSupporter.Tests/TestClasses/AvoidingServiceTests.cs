using FluentAssertions;
using TractorSupporter.Tests.Fixtures;

namespace TractorSupporter.Tests.TestClasses;

[Collection("Services")]
public class AvoidingServiceTests
{
    private readonly ServicesFixture _servicesFixture;

    public AvoidingServiceTests(ServicesFixture servicesFixture)
    {
        _servicesFixture = servicesFixture;
    }

    [Theory]
    [InlineData(1000.0, 500)]
    [InlineData(3000.0, 500)]
    [InlineData(2000.0, 500)]
    [InlineData(4000.0, 500)]
    [InlineData(8000.0, 500)]
    public void MakeAvoidingDecisionForSmallerAvoidingDistance_ReturnsTrue(double distanceMeasured, double avoidingDistance)
    {
        // Arrange
        var service = _servicesFixture.AvoidingService;
        service.AvoidingDistance = avoidingDistance;

        // Act
        var decision = service.MakeAvoidingDecision(distanceMeasured);

        // Assert
        decision.Should().BeFalse();
    }
}
