// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Its.Log.Instrumentation;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Xunit;

    using static System.FormattableString;

    public static class InMemoryTests
    {
        [Fact]
        public static void LogConfigurationInMemoryConstructor___Less_than_NegativeOne_MaxLoggedItemCount___Throws()
        {
            // Arrange
            Action action = () => new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, -2);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.Message.Should().Be("Specified argument was out of the range of valid values.\r\nParameter name: maxLoggedItemCount is <= -1; value is -2");
        }

        [Fact]
        public static void LogProcessorInMemoryConstructor___Null_config___Throws()
        {
            // Arrange
            Action action = () => new InMemoryLogWriter(null);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("Value cannot be null.\r\nParameter name: logWriterConfigBase");
        }

        [Fact]
        public static void LogProcessor___Valid___Works()
        {
            // Arrange
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere);
            var processor = new InMemoryLogWriter(configuration);

            var infoCanary = A.Dummy<string>();
            var errorCanary = new ArgumentException(A.Dummy<string>());

            // Act
            processor.Log(infoCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));
            processor.Log(errorCanary.ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted));

            // Assert
            processor.LoggedItems.Count.Should().Be(2);
            processor.LoggedItems.Single(_ => _.Context.Origin == LogItemOrigin.ItsLogEntryPosted.ToString() && _.Kind == LogItemKind.String).Subject.Summary.Should().Contain(infoCanary);
            processor.LoggedItems.Single(_ => _.Context.Origin == LogItemOrigin.ItsLogEntryPosted.ToString() && _.Kind == LogItemKind.Exception).Subject.Summary.Should().Contain(errorCanary.Message);
        }

        [Fact]
        public static void LogProcessor___Default_max_elements___Honored()
        {
            // Arrange
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log("Hello".ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted)));

            // Assert
            processor.LoggedItems.Count.Should().Be(logCallCount);
        }

        [Fact]
        public static void LogProcessor___0_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 0;
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log("Hello".ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted)));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___1_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 1;
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log("Hello".ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted)));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___2_max_elements___Honored()
        {
            // Arrange
            var maxLoggedItemCount = 2;
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere, maxLoggedItemCount);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 1000;

            // Act
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log("Hello".ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted)));

            // Assert
            processor.LoggedItems.Count.Should().Be(maxLoggedItemCount);
        }

        [Fact]
        public static void LogProcessor___Purge___Works()
        {
            // Arrange
            var configuration = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere);
            var processor = new InMemoryLogWriter(configuration);
            var logCallCount = 10;
            Enumerable.Range(0, logCallCount).ToList().ForEach(_ => processor.Log("Hello".ToLogEntry().ToLogItem(LogItemOrigin.ItsLogEntryPosted)));

            // Act
            processor.PurgeAllLoggedItems();

            // Assert
            processor.LoggedItems.Count.Should().Be(0);
        }

        [Fact]
        public static void RoundtripSerialization___LogConfigurationInMemory___Works()
        {
            // Arrange
            var expected = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.AnythingFromAnywhere);

            void ThrowIfObjectsDiffer(object actualAsObject)
            {
                var actual = actualAsObject as InMemoryLogConfig;
                actual.Should().NotBeNull();
                actual.Should().Be(expected);
            }

            // Act & Assert
            ShareSerializationTestLogic.ActAndAssertForRoundtripSerialization(expected, ThrowIfObjectsDiffer);
        }

        [Fact]
        public static void FileConfiguration___EqualityLogic___Should_be_valid___When_different_data()
        {
            // Arrange
            var context = LogInclusionKindToOriginsMaps.AnythingFromAnywhere;
            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.ExceptionsFromAnywhere, 1),
                                                Second = new InMemoryLogConfig(LogInclusionKindToOriginsMaps.StringAndObjectsFromItsLogEntryPosted, 1),
                                            },
                                        new
                                            {
                                                First = new InMemoryLogConfig(context, 1),
                                                Second = new InMemoryLogConfig(context, 0),
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
        public static void FileConfiguration___EqualityLogic___Should_be_valid___When_same_data()
        {
            // Arrange
            var logContexts = LogInclusionKindToOriginsMaps.AnythingFromAnywhere;

            var notEqualTests = new[]
                                    {
                                        new
                                            {
                                                First = new InMemoryLogConfig(logContexts, 1),
                                                Second = new InMemoryLogConfig(logContexts, 1),
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
