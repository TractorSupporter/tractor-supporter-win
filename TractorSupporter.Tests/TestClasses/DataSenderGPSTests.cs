using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using TractorSupporter.Services;

namespace TractorSupporter.Tests.TestClasses
{
    

    namespace TractorSupporter.Tests
    {
        public class DataSenderGPSTests
        {
            private readonly IDataSenderGPS _dataSenderGPS;
            private readonly IGPSConnectionService _fakeGpsConnectionService;

            public DataSenderGPSTests()
            {
                _fakeGpsConnectionService = A.Fake<IGPSConnectionService>();
                _dataSenderGPS = new DataSenderGPS(_fakeGpsConnectionService);
            }

            [Fact]
            public async Task SendData_ShouldNotSend_WhenConnectingToGPSIsNotAllowed()
            {
                // Arrange
                var jsonData = new { allowAvoidingDecision = true };
                A.CallTo(() => _fakeGpsConnectionService.ConnectingToGPSAllowed).Returns(false); // No connection allowed

                // Act
                await _dataSenderGPS.SendData(jsonData);

                // Assert
                A.CallTo(() => _fakeGpsConnectionService.WriteToPipe(A<string>.Ignored)).MustNotHaveHappened(); // WriteToPipe should not be called
            }

            [Fact]
            public async Task SendData_ShouldSendMessage_WhenConnectedToGPS()
            {
                // Arrange
                var jsonData = new { allowAvoidingDecision = true };
                var jsonString = JsonSerializer.Serialize(jsonData);
                A.CallTo(() => _fakeGpsConnectionService.IsConnectedToGPS).Returns(true); // GPS is connected
                A.CallTo(() => _fakeGpsConnectionService.ConnectingToGPSAllowed).Returns(true); // Connection is allowed

                // Act
                await _dataSenderGPS.SendData(jsonData);

                // Assert
                A.CallTo(() => _fakeGpsConnectionService.WriteToPipe(jsonString)).MustHaveHappened(); // WriteToPipe should be called with correct data
            }

        }
    }

}
