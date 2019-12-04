// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidLogEntryTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Diagnostics;

    using FakeItEasy;

    using FluentAssertions;

    using Its.Log.Instrumentation;
    using Its.Log.Instrumentation.Extensions;

    using Naos.Logging.Domain;

    using OBeautifulCode.Reflection.Recipes;

    using Xunit;

    using static System.FormattableString;

    public static class InvalidLogEntryTests
    {
        [Fact]
        public static void Category_property_set___Should_throw()
        {
            // Arrange
            var category = A.Dummy<string>();
            var badLogEntry = new LogEntry("Subject") { Category = category, };

            // Act
            var exception = Record.Exception(() => badLogEntry.ToLogItem(LogItemOrigin.ItsLogEntryPosted));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be(Invariant($"LogEntry cannot have the property Category set; found: {category}."));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void Params_property_set___Should_throw___When_not_LogActivity_trace()
        {
            // Arrange
            var param = new Params<string>();
            var paramsString = A.Dummy<string>();
            Func<string> paramsAccessor = () => paramsString;
            param.SetFieldValue("paramsAccessor", paramsAccessor);
            var badLogEntry = new LogEntry("Subject");
            badLogEntry.SetExtension(param);

            // Act
            var exception = Record.Exception(() => badLogEntry.ToLogItem(LogItemOrigin.ItsLogEntryPosted));

            // Assert
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be(Invariant($"LogEntry cannot have the property Params set unless it's part of the Trace scenario; found: {paramsString}"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void Params_property_set_with_too_many_values___Should_throw___When_in_LogActivity_trace()
        {
            // Arrange
            var badLogEntry = new LogEntry("Subject") { EventType = TraceEventType.Verbose };

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            badLogEntry.SetExtension(stopwatch);

            var paramOne = new Params<string>();
            var paramsOneString = A.Dummy<string>();
            string ParamsOneAccessor() => paramsOneString;
            paramOne.SetFieldValue("paramsAccessor", (Func<string>)ParamsOneAccessor);
            badLogEntry.SetExtension(paramOne);

            var paramTwo = new Params<object>();
            var paramsTwoString = A.Dummy<string>();
            string ParamsTwoAccessor() => paramsTwoString;
            paramTwo.SetFieldValue("paramsAccessor", (Func<object>)ParamsTwoAccessor);
            badLogEntry.SetExtension(paramTwo);

            // Act
            var exception = Record.Exception(() => badLogEntry.ToLogItem(LogItemOrigin.ItsLogEntryPosted));

            // Assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be(Invariant($"LogEntry cannot have the property Params set with more than one value as part of the Trace scenario; found: {paramsOneString},{paramsTwoString}"));
        }

        [Fact]
        public static void ElapsedMilliseconds_property_not_set___Should_throw___When_in_LogActivity_trace()
        {
            // Arrange
            var badLogEntry = new LogEntry("Subject") { EventType = TraceEventType.Verbose };

            var param = new Params<string>();
            var paramsString = A.Dummy<string>();
            string ParamsAccessor() => paramsString;
            param.SetFieldValue("paramsAccessor", (Func<string>)ParamsAccessor);
            badLogEntry.SetExtension(param);

            // Act
            var exception = Record.Exception(() => badLogEntry.ToLogItem(LogItemOrigin.ItsLogEntryPosted));

            // Assert
            exception.Should().BeOfType<InvalidOperationException>();
            exception.Message.Should().Be(Invariant($"logEntry.ElapsedMilliseconds is null when there is an ElapsedCorrelation"));
        }
    }
}
