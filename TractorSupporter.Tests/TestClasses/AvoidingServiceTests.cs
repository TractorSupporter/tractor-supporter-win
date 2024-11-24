using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services;
using Xunit;

namespace TractorSupporter.Tests
{
    public class AvoidingServiceTests
    {
        ILoggingService _loggingService;

        public AvoidingServiceTests()
        {
            _loggingService = A.Fake<ILoggingService>();
            A.CallTo(() => _loggingService.AddLog(A<DecisionType>.Ignored)).DoesNothing();
        }

        [Fact]
        public void AllowMakingDecision_ShouldUpdateDecisionAllowedFlag()
        {
            // Arrange
            var avoidingService = new AvoidingService(_loggingService);

            // Act
            avoidingService.AllowMakingDecision(null, true);

            // Assert
            GetPrivateField<bool>(avoidingService, "_avoidingDecisionAllowed")
                .Should().BeTrue();
        }

        [Fact]
        public void MakeAvoidingDecision_ShouldReturnTrue_WhenConditionsMet()
        {
           // Arrange
           var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDistanceTimes", new List<DateTime>());
            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", true);
            SetPrivateField(avoidingService, "_minAvoidingSignalsCount", 1);
            SetPrivateField(avoidingService, "_avoidingDistanceSignalValidLifetimeMs", 1000);

            avoidingService.AvoidingDistance = 10.0;

            var distanceMeasured = 9.0;


            // Act
            var result = avoidingService.MakeAvoidingDecision(distanceMeasured);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void MakeAvoidingDecision_ShouldReturnFalse_WhenDecisionNotAllowed()
        {
            // Arrange
            
            var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", false);

            // Act
            var result = avoidingService.MakeAvoidingDecision(5.0);

            // Assert
            result.Should().BeFalse();
        }

        #region Helper Methods

        private void ReplaceLoggingServiceWithFake()
        {
            var fakeLoggingService = A.Fake<LoggingService>();
            var field = typeof(LoggingService)
                .GetField("_instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (field == null)
                throw new InvalidOperationException("The _instance field was not found in LoggingService.");

            field.SetValue(null, fakeLoggingService);
        }

        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType()
                .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
                throw new InvalidOperationException($"Field {fieldName} was not found.");

            return (T)field.GetValue(obj)!;
        }

        private void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType()
                .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
                throw new InvalidOperationException($"Field {fieldName} was not found.");

            field.SetValue(obj, value);
        }

        #endregion
    }
}
