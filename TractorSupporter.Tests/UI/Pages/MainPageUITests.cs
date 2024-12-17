using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System.Windows;
using Application = FlaUI.Core.Application;

namespace TractorSupporter.Tests.E2E.Pages;

[CollectionDefinition("SequentialTestCollection")]
public class MainPageUITests : IDisposable
{
    private readonly Application _application;
    private readonly AutomationBase _automation;
    private readonly FlaUI.Core.AutomationElements.Window mainWindow;

    public MainPageUITests()
    {
        //UITestHelpers.DeletePreviousProcesses();
        _application = Application.Launch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TractorSupporter.exe"));
        _automation = new UIA3Automation();

        UITestHelpers.DeleteConfig();
        mainWindow = _application.GetMainWindow(_automation);
        UITestHelpers.SkipStarterWindow(mainWindow);
    }

    public void Dispose()
    {
        _automation.Dispose();
        if (!_application.HasExited)
        {
            _application.Close();
        }
    }


    [Fact]
    public void StartConnection_ChangesButtonState()
    {
        // Arrange
        var connectButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("ConnectButton")).AsButton();
        Assert.NotNull(connectButton);

        // Act
        Retry.WhileFalse(() => connectButton.IsEnabled, TimeSpan.FromSeconds(5));
        connectButton.Invoke();

        // Assert
        Assert.Equal("Stop Connection", connectButton.Properties.Name.Value);
        Dispose();
    }
}