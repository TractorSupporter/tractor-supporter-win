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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AllowMakingDecision_ShouldUpdateDecisionAllowedFlag(bool expectedValue)
        {
            // Arrange
            var avoidingService = new AvoidingService(_loggingService);

            // Act
            avoidingService.AllowMakingDecision(null, expectedValue);

            // Assert
            GetPrivateField<bool>(avoidingService, "_avoidingDecisionAllowed")
                .Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(true, 1, 1000, 10, 9)]
        [InlineData(true, 2, 1000, 10, 9)]
        [InlineData(true, 3, 1000, 10, 9)]
        public void MakeAvoidingDecision_ShouldReturnTrue_WhenConditionsMet(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double avoidingDist, double measuredDist)
        {
           // Arrange
           var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDistanceTimes", new List<DateTime>());
            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", allowedDecision);
            SetPrivateField(avoidingService, "_minAvoidingSignalsCount", minSignalsCount);
            SetPrivateField(avoidingService, "_avoidingDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            avoidingService.AvoidingDistance = avoidingDist;

            var distanceMeasured = measuredDist;

            bool result = false;
            // Act
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = avoidingService.MakeAvoidingDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(false, 1, 1000, 10, 9)]
        [InlineData(false, 2, 1000, 10, 11)]
        [InlineData(false, 3, 1000, 10, 9)]
        public void MakeAvoidingDecision_ShouldReturnFalse_WhenDecisionNotAllowed(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double avoidingDist, double measuredDist)
        {
            // Arrange
            
            var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDistanceTimes", new List<DateTime>());
            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", allowedDecision);
            SetPrivateField(avoidingService, "_minAvoidingSignalsCount", minSignalsCount);
            SetPrivateField(avoidingService, "_avoidingDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            avoidingService.AvoidingDistance = avoidingDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            // Act
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = avoidingService.MakeAvoidingDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, 1, 100, 10, 200)]
        [InlineData(true, 2, 100, 10, 11)]
        [InlineData(true, 3, 100, 10, 111)]
        public void MakeAvoidingDecision_ShouldReturnFalse_DistanceTooHigh(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double avoidingDist, double measuredDist)
        {
            // Arrange

            var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDistanceTimes", new List<DateTime>());
            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", allowedDecision);
            SetPrivateField(avoidingService, "_minAvoidingSignalsCount", minSignalsCount);
            SetPrivateField(avoidingService, "_avoidingDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            avoidingService.AvoidingDistance = avoidingDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            // Act
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = avoidingService.MakeAvoidingDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, 2, 100, 10, 1)]
        [InlineData(true, 3, 100, 10, 1)]
        [InlineData(true, 4, 100, 10, 1)]
        public void MakeAvoidingDecision_ShouldReturnFalse_SignalsTimeout(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double avoidingDist, double measuredDist)
        {
            // Arrange

            var avoidingService = new AvoidingService(_loggingService);

            SetPrivateField(avoidingService, "_avoidingDistanceTimes", new List<DateTime>());
            SetPrivateField(avoidingService, "_avoidingDecisionAllowed", allowedDecision);
            SetPrivateField(avoidingService, "_minAvoidingSignalsCount", minSignalsCount);
            SetPrivateField(avoidingService, "_avoidingDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            avoidingService.AvoidingDistance = avoidingDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            // Act
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = avoidingService.MakeAvoidingDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs + minSignalsCount * 2);
            }

            // Assert
            result.Should().BeFalse();
        }

        #region Helper Methods

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
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field == null) throw new InvalidOperationException($"Field {fieldName} not found.");

            field.SetValue(obj, value);
        }

        #endregion
    }
}
