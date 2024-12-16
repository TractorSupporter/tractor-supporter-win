
namespace TractorSupporter.Tests.UI;

public class TestFixture : IDisposable
{
    public TestFixture()
    {
        // Shared setup logic (e.g., start application, prepare resources)
        Console.WriteLine("Shared setup");
    }

    public void Dispose()
    {
        // Shared cleanup logic
        Console.WriteLine("Shared cleanup");
    }
}
