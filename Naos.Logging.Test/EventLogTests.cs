﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Xunit;

    using static System.FormattableString;

    public static class EventLogTests
    {
        [Fact]
        public static void EventLogConfigurationConstructor___Valid___Works()
        {
            // Arrange && Act
            var actual = new EventLogConfig(new Dictionary<LogItemKind, IReadOnlyCollection<string>>());

            // Assert
            actual.Should().NotBeNull();
            actual.LogName.Should().Be(EventLogConfig.DefaultLogName);
            actual.MachineName.Should().Be(EventLogConfig.DefaultMachineName);
            actual.Source.Should().Be(Process.GetCurrentProcess().ProcessName);
            actual.ShouldCreateSourceIfMissing.Should().Be(false);
        }

        [Fact]
        public static void EventLogConfigurationConstructor___Valid_override_defaults___Works()
        {
            // Arrange
            var contextsToLog = new Dictionary<LogItemKind, IReadOnlyCollection<string>>();
            var logName = A.Dummy<string>();
            var machineName = A.Dummy<string>();
            var source = A.Dummy<string>();
            var shouldCreateSource = true;

            // Act
            var actual = new EventLogConfig(contextsToLog, source, logName, machineName, shouldCreateSource);

            // Assert
            actual.Should().NotBeNull();
            actual.LogName.Should().Be(logName);
            actual.MachineName.Should().Be(machineName);
            actual.Source.Should().Be(source);
            actual.ShouldCreateSourceIfMissing.Should().Be(shouldCreateSource);
        }

        [Fact]
        public static void LogProcessorEventLogConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new EventLogWriter(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Value cannot be null.\r\nParameter name: logWriterConfigBase");
        }

        [Fact]
        public static void RoundtripSerialization___EventLogConfiguration___Works()
        {
            // Arrange
            var expected = ShareSerializationTestLogic.EventLogConfig;

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as EventLogConfig;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ShareSerializationTestLogic.ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void EventLogConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var logContexts = LogInclusionKindToOriginsMaps.TelemetryFromAnywhere;
            var logName = A.Dummy<string>();
            var machineName = A.Dummy<string>();
            var source = A.Dummy<string>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new EventLogConfig(LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted, source: source, logName: logName, machineName: machineName),
                                                Second = new EventLogConfig(LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere, source: source, logName: logName, machineName: machineName),
                                            },
                                        new
                                            {
                                                First = new EventLogConfig(logContexts, source: source, logName: "Log1", machineName: machineName),
                                                Second = new EventLogConfig(logContexts, source: source, logName: "Log2", machineName: machineName),
                                            },
                                        new
                                            {
                                                First = new EventLogConfig(logContexts, source: source, shouldCreateSourceIfMissing: true, logName: logName, machineName: machineName),
                                                Second = new EventLogConfig(logContexts, source: source, shouldCreateSourceIfMissing: false, logName: logName, machineName: machineName),
                                            },
                                        new
                                            {
                                                First = new EventLogConfig(logContexts, source: source, logName: logName, machineName: "Machine1"),
                                                Second = new EventLogConfig(logContexts, source: source, logName: logName, machineName: "Machine2"),
                                            },
                                        new
                                            {
                                                First = new EventLogConfig(logContexts, source: "Source1", logName: logName, machineName: machineName),
                                                Second = new EventLogConfig(logContexts, source: "Source2", logName: logName, machineName: machineName),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals(_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }

        [Fact]
        public static void EventLogConfiguration___EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var logContexts = LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere;
            var logName = A.Dummy<string>();
            var machineName = A.Dummy<string>();
            var source = A.Dummy<string>();

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new EventLogConfig(logContexts, source: source, logName: logName, machineName: machineName),
                                                Second = new EventLogConfig(logContexts, source: source, logName: logName, machineName: machineName),
                                            },
                                    }.ToList();

            // Act & Assert
            notEqualTests.ForEach(
                _ =>
                    {
                        if (_.First != null && _.Second != null)
                        {
                            _.First.Equals(_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            _.First.Equals((object)_.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                            (_.First.GetHashCode() == _.Second.GetHashCode()).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        }

                        (_.First == _.Second).Should().BeTrue(Invariant($"First: {_.First}; Second: {_.Second}"));
                        (_.First != _.Second).Should().BeFalse(Invariant($"First: {_.First}; Second: {_.Second}"));
                    });
        }
    }
}
