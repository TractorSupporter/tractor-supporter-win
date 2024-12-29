using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services;
using Xunit;

namespace TractorSupporter.Tests
{
    public class AlarmServiceTests
    {
        private readonly ILoggingService _loggingService;

        public AlarmServiceTests()
        {
            _loggingService = A.Fake<ILoggingService>();
            A.CallTo(() => _loggingService.AddLog(A<DecisionType>.Ignored)).DoesNothing();
            _loggingService = A.Fake<ILoggingService>();
            A.CallTo(() => _loggingService.AddLog(A<DecisionType>.Ignored)).DoesNothing();
        }

        [Theory]
        [InlineData(true, 1, 1000, 10, 9)]
        [InlineData(true, 2, 1000, 10, 9)]
        [InlineData(true, 3, 1000, 10, 9)]
        public void MakeAlarmDecision_ShouldReturnTrue_WhenConditionsMet(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double alarmDist, double measuredDist)
        {
            // Arrange
            var alarmService = new AlarmService(_loggingService);

            SetPrivateField(alarmService, "_alarmDistanceTimes", new List<DateTime>());
            SetPrivateField(alarmService, "_alarmDecisionAllowed", allowedDecision);
            SetPrivateField(alarmService, "_minAlarmSignalsCount", minSignalsCount);
            SetPrivateField(alarmService, "_alarmDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            alarmService.AlarmDistance = alarmDist;

            var distanceMeasured = measuredDist;

            bool result = false;
            // Act
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = alarmService.MakeAlarmDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(false, 1, 100, 10, 9)]
        [InlineData(false, 2, 100, 10, 11)]
        [InlineData(false, 3, 100, 10, 9)]
        public void MakeAlarmDecision_ShouldReturnFalse_WhenDecisionNotAllowed(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double alarmDist, double measuredDist)
        {
            // Arrange
            var alarmService = new AlarmService(_loggingService);

            SetPrivateField(alarmService, "_alarmDistanceTimes", new List<DateTime>());
            SetPrivateField(alarmService, "_alarmDecisionAllowed", allowedDecision);
            SetPrivateField(alarmService, "_minAlarmSignalsCount", minSignalsCount);
            SetPrivateField(alarmService, "_alarmDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            alarmService.AlarmDistance = alarmDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = alarmService.MakeAlarmDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, 1, 100, 10, 200)]
        [InlineData(true, 2, 100, 10, 11)]
        [InlineData(true, 3, 100, 10, 111)]
        public void MakeAlarmDecision_ShouldReturnFalse_WhenDistanceTooHigh(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double alarmDist, double measuredDist)
        {
            // Arrange
            var alarmService = new AlarmService(_loggingService);

            SetPrivateField(alarmService, "_alarmDistanceTimes", new List<DateTime>());
            SetPrivateField(alarmService, "_alarmDecisionAllowed", allowedDecision);
            SetPrivateField(alarmService, "_minAlarmSignalsCount", minSignalsCount);
            SetPrivateField(alarmService, "_alarmDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            alarmService.AlarmDistance = alarmDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = alarmService.MakeAlarmDecision(distanceMeasured);
                Thread.Sleep(signalValidLifetimeMs / minSignalsCount);
            }

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, 2, 100, 10, 1)]
        [InlineData(true, 3, 100, 10, 1)]
        [InlineData(true, 4, 100, 10, 1)]
        public void MakeAlarmDecision_ShouldReturnFalse_WhenSignalsTimeout(bool allowedDecision, int minSignalsCount, int signalValidLifetimeMs, double alarmDist, double measuredDist)
        {
            // Arrange
            var alarmService = new AlarmService(_loggingService);

            SetPrivateField(alarmService, "_alarmDistanceTimes", new List<DateTime>());
            SetPrivateField(alarmService, "_alarmDecisionAllowed", allowedDecision);
            SetPrivateField(alarmService, "_minAlarmSignalsCount", minSignalsCount);
            SetPrivateField(alarmService, "_alarmDistanceSignalValidLifetimeMs", signalValidLifetimeMs);

            alarmService.AlarmDistance = alarmDist;

            var distanceMeasured = measuredDist;

            // Act
            bool result = false;
            for (int i = 0; i < minSignalsCount; i++)
            {
                result = alarmService.MakeAlarmDecision(distanceMeasured);
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
