
using FlaUI.Core.AutomationElements;
using System.Diagnostics;
using TractorSupporter.Tests.UI;

namespace TractorSupporter.Tests.E2E;

public static class UITestHelpers
{
    public const string PORT_EXPECTED_ERROR = "Port is invalid. It must be a number between 1 and 65535.";
    public const string IP_EXPECTED_ERROR = "IP address is invalid.";

    public static void SkipStarterWindow(Window mainWindow)
    {
        var acceptButton = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("Accept")).AsButton();
        if (acceptButton == null)
        {
            return;
        }

        acceptButton.Invoke();
        Thread.Sleep(1000);
    }

    public static void DeleteConfig()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public static AutomationElement FindElementByIdAndAssert(Window mainWindow, string automationId)
    {
        return Retry.For(() =>
        {
            // Attempt to find the element
            var element = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId));
            Assert.NotNull(element); // Assert that the element is not null
            return element;
        }, TimeSpan.FromSeconds(10)); // Retry for up to 10 seconds
    }

    public static void DeletePreviousProcesses()
    {
        foreach (var process in Process.GetProcessesByName("TractorSupporter"))
        {
            try
            {
                process.Kill();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to kill process TractorSupporter: {ex.Message}");
            }
        }
    }
}
