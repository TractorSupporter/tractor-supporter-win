using TractorSupporter.Services;

namespace TractorSupporter.Tests.Fixtures;

[CollectionDefinition("Services")]
public class ServicesFixtureCollection : ICollectionFixture<ServicesFixture> {}

public class ServicesFixture
{
    public AvoidingService AvoidingService => AvoidingService.Instance;
    public AlarmService AlarmService => AlarmService.Instance;
}
