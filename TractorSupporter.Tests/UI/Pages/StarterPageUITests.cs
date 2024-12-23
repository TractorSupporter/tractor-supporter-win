﻿using FlaUI.Core.AutomationElements;
using FlaUI.Core;
using FlaUI.UIA3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Tests.E2E;
using FlaUI.Core.Tools;

namespace TractorSupporter.Tests.E2E.Pages;

[CollectionDefinition("SequentialTestCollection")]
public class StarterPageUITests : IDisposable
{
    private readonly Application _application;
    private readonly AutomationBase _automation;
    private readonly FlaUI.Core.AutomationElements.Window mainWindow;
    private Button connectButton;
    private TextBox portTextBox;
    private TextBox ipTextBox;
    private TextBox portTextBoxError;
    private TextBox ipTextBoxError;

    public StarterPageUITests()
    {
        //UITestHelpers.DeletePreviousProcesses();
        _application = Application.Launch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TractorSupporter.exe"));
        _automation = new UIA3Automation();

        UITestHelpers.DeleteConfig();
        mainWindow = _application.GetMainWindow(_automation);

        Thread.Sleep(1000);
        // Arrange
        connectButton = UITestHelpers.FindElementByIdAndAssert(mainWindow, "Accept").AsButton();
        portTextBox = UITestHelpers.FindElementByIdAndAssert(mainWindow, "PortTextBox").AsTextBox();
        ipTextBox = UITestHelpers.FindElementByIdAndAssert(mainWindow, "IpTextBox").AsTextBox();
        portTextBoxError = UITestHelpers.FindElementByIdAndAssert(mainWindow, "PortTextBoxError").AsTextBox();
        ipTextBoxError = UITestHelpers.FindElementByIdAndAssert(mainWindow, "IpTextBoxError").AsTextBox();
    }

    public void Dispose()
    {
        _automation.Dispose();
        if (!_application.HasExited)
        {
            _application.Close();
        }
    }


    [Theory]
    [InlineData("", "", UITestHelpers.PORT_EXPECTED_ERROR, UITestHelpers.IP_EXPECTED_ERROR)]
    [InlineData("80", "", "", UITestHelpers.IP_EXPECTED_ERROR)]
    [InlineData("", "0.0.0.0", UITestHelpers.PORT_EXPECTED_ERROR, "")]
    public void MoveNext_ChangesButtonState(string portInput, string ipInput, string portExpectedError, string ipExpectedError)
    {
        Thread.Sleep(1000);
        portTextBox.Enter(portInput);
        ipTextBox.Enter(ipInput);

        // Act
        Thread.Sleep(1000);
        Retry.WhileFalse(() => connectButton.IsEnabled, TimeSpan.FromSeconds(5));
        connectButton.Invoke();

        // Assert
        Thread.Sleep(1000);
        Assert.Equal(portExpectedError, portTextBoxError.Properties.Name.Value);
        Assert.Equal(ipExpectedError, ipTextBoxError.Properties.Name.Value);
        Dispose();
    }
}