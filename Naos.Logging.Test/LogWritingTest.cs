﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWritingTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Naos.Serialization.Json;
    using OBeautifulCode.Error.Recipes;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    public class LogWritingTest
    {
        private readonly ITestOutputHelper testOutputHelper;
        private static InMemoryLogWriter memoryLogWriter;
        private static string expectedOrigin = LogItemOrigin.NaosLoggingLogger.ToString();

        public LogWritingTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Debug()
        {
            var config = new EventLogConfig(
                new Dictionary<LogItemKind, IReadOnlyCollection<string>>
                {
                    { LogItemKind.String, null }, { LogItemKind.Object, null }, { LogItemKind.Exception, null },
                    { LogItemKind.Unknown, null }, { LogItemKind.Telemetry, null },
                },
                shouldCreateSourceIfMissing: true,
                logName: "CoMetrics",
                source: "CmLogShipper");
            var json = new NaosJsonSerializer().SerializeToString(config);
            this.testOutputHelper.WriteLine(json);
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_string_subject_via_lambda_without_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = "this is a string subject " + A.Dummy<string>();

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<string>();

            logItem.Subject.Summary.Should().Be(subject);
            actualSubject.Should().Be(subject);
            logItem.Kind.Should().Be(LogItemKind.String);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_real_object_subject_via_lambda_with_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = new TestObjectWithToString();
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<TestObjectWithToString>();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            actualSubject.ToString().Should().Be(subject.ToString());
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_real_object_subject_via_lambda_without_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = new TestObjectWithToString();

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<TestObjectWithToString>();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            actualSubject.ToString().Should().Be(subject.ToString());
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_anonymous_object_subject_via_lambda_with_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = new { Message = "hello" };
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            dynamic actualSubject = logItem.Subject.DeserializeSubject();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            ((string)actualSubject.Message).Should().Be(subject.Message);
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_anonymous_object_subject_via_lambda_without_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = new { Message = "hello" };

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            dynamic actualSubject = logItem.Subject.DeserializeSubject();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            ((string)actualSubject.Message).Should().Be(subject.Message);
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_string_subject_via_lambda_with_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var subject = "this is a string subject " + A.Dummy<string>();
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<string>();

            logItem.Subject.Summary.Should().Be(subject);
            actualSubject.Should().Be(subject);
            logItem.Kind.Should().Be(LogItemKind.String);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public static void Log_Write___Records_details_correctly___With_exception_and_no_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var exceptionMessage = "Exception " + A.Dummy<string>();

            // Act
            try
            {
                throw new InvalidOperationException(exceptionMessage);
            }
            catch (Exception ex)
            {
                Log.Write(() => ex);
            }

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single();
            exceptionCorrelation.CorrelationId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [Fact]
        public static void Log_Write___Records_details_correctly___With_exception_and_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var exceptionMessage = "Exception " + A.Dummy<string>();
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            try
            {
                throw new InvalidOperationException(exceptionMessage);
            }
            catch (Exception ex)
            {
                Log.Write(() => ex, comment);
            }

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single();
            exceptionCorrelation.CorrelationId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need lowercase.")]
        [Fact]
        public static void Log_Write___Records_details_correctly___With_exception_and_comment_and_error_code()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var exceptionMessage = "Exception " + A.Dummy<string>();
            var comment = "this is a comment " + A.Dummy<string>();
            var errorCode = "errorCode" + Guid.NewGuid().ToString().ToLowerInvariant();

            // Act
            try
            {
                throw new InvalidOperationException(exceptionMessage).AddErrorCode(errorCode);
            }
            catch (Exception ex)
            {
                Log.Write(() => ex, comment);
            }

            // Assert
            var logItem = logWriter.LoggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeBefore(DateTime.UtcNow);

            var errorCodeCorrelation = (ErrorCodeCorrelation)logItem.Correlations.Single(_ => _.GetType() == typeof(ErrorCodeCorrelation));
            errorCodeCorrelation.ErrorCode.Should().Be(errorCode);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single(_ => _.GetType() == typeof(ExceptionIdCorrelation));
            exceptionCorrelation.CorrelationId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [Fact]
        public static void Log_Write___Records_details_correctly___With_exception_and_inner_exception()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();
            var exceptionMessage = "Exception " + A.Dummy<string>();
            var innerExceptionMessage = "Exception to inner " + A.Dummy<string>();

            // Act
            try
            {
                try
                {
                    throw new InvalidOperationException(innerExceptionMessage);
                }
                catch (Exception innerException)
                {
                    Log.Write(() => innerException);
                    throw new InvalidOperationException(exceptionMessage, innerException);
                }
            }
            catch (Exception exception)
            {
                Log.Write(() => exception);
            }

            // Assert
            var timeOrderedItems = logWriter.LoggedItems.OrderBy(_ => _.Context.TimestampUtc).ToList();
            timeOrderedItems.Count.Should().Be(2);
            var logItemInner = logWriter.LoggedItems.First();
            var logItem = logWriter.LoggedItems.Last();

            var innerSubject = logItemInner.Subject.DeserializeSubject<InvalidOperationException>();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItemInner.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {innerExceptionMessage}"));
            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));

            innerSubject.Message.Should().Be(innerExceptionMessage);
            innerSubject.InnerException.Should().BeNull();

            actualSubject.Message.Should().Be(exceptionMessage);
            actualSubject.InnerException.Should().NotBeNull();
            actualSubject.InnerException.Message.Should().Be(innerExceptionMessage);

            logItemInner.Kind.Should().Be(LogItemKind.Exception);
            logItemInner.Comment.Should().BeNull();

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().BeNull();

            var innerCorrelation = (ExceptionIdCorrelation)logItemInner.Correlations.Single();
            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single();

            innerCorrelation.CorrelationId.Should().Be(innerSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
            exceptionCorrelation.CorrelationId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());

            innerCorrelation.CorrelationId.Should().Be(exceptionCorrelation.CorrelationId);
        }

        [Fact]
        public static void Log_Enter_Activity_Trace___Records_details_correctly___With_exception_and_comment()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();

            var enterSubject = new TestObjectWithToString();
            var stringTraceWithoutLambda = "some trace without a lambda" + A.Dummy<string>();
            var stringTraceWithLambda = "some trace with a lambda" + A.Dummy<string>();
            var traceObjectWithLambda = new DifferentTestObjectWithToString();
            var exceptionToThrow = new InvalidOperationException("Oh no.");

            // Act
            using (var log = Log.GetUsingBlockLogger(() => enterSubject))
            {
                // to make sure the next message doesn't come too quickly for the assert later.
                Thread.Sleep(1);
                try
                {
                    throw exceptionToThrow;
                }
                catch (Exception exception)
                {
                    log.Write(() => exception);
                }

                log.Write(() => stringTraceWithLambda);
                log.Write(() => traceObjectWithLambda);
            }

            // Assert
            //logWriter.LoggedItems.ToList().ForEach(_ => _.Correlations.Single(c => c is ActivityCorrelation).Should().NotBeNull());
            //logWriter.LoggedItems.Select(_ => _.Correlations.Single(c => c is ActivityCorrelation).CorrelationId).Distinct().Count().Should()
            //    .Be(1);
            //logWriter.LoggedItems.ToList().ForEach(
            //    _ => ((ActivityCorrelation)_.Correlations.Single(c => c is ActivityCorrelation)).CorrelatingSubject
            //        .DeserializeSubject<TestObjectWithToString>().Test.Should().Be(enterSubject.Test));

            //var enterItem = logWriter.LoggedItems.Single(_ => _.Subject.Summary.StartsWith(ActivityCorrelationPosition.First.ToString(), StringComparison.CurrentCulture));
            //enterItem.Subject.DeserializeSubject<TestObjectWithToString>().Test.Should().Be(enterSubject.Test);
            //var enterCorelation = (ActivityCorrelation)enterItem.Correlations.Single(_ => _ is ActivityCorrelation);
            //enterCorelation.ElapsedMillisecondsFromFirst.Should().Be(0);

            //var middleItems = logWriter.LoggedItems.Where(
            //    _ => new[] { exceptionToThrow.Message, stringTraceWithLambda, stringTraceWithoutLambda, traceObjectWithLambda.ToString() }.Any(
            //        a => _.Subject.Summary.EndsWith(a, StringComparison.CurrentCulture))).ToList();
            //middleItems.ForEach(_ =>
            //    {
            //        var middleCorrelation = (ActivityCorrelation)_.Correlations.Single(s => s is ActivityCorrelation);
            //        middleCorrelation.ElapsedMillisecondsFromFirst.Should().BeGreaterThan(0);
            //    });

            //var exitItem = logWriter.LoggedItems.Single(_ => _.Subject.Summary.StartsWith(ActivityCorrelationPosition.Last.ToString(), StringComparison.CurrentCulture));
            //exitItem.Subject.DeserializeSubject<TestObjectWithToString>().Test.Should().Be(enterSubject.Test);
            //var exitCorelation = (ActivityCorrelation)exitItem.Correlations.Single(_ => _ is ActivityCorrelation);
            //exitCorelation.ElapsedMillisecondsFromFirst.Should().BeGreaterThan(0);
        }

        [Fact]
        public static void Log_Write___Should_make_null_subject_LogItem___When_nulls_written()
        {
            // Arrange
            var logWriter = BuildAndConfigureMemoryLogWriter();

            // Act
            Log.Write(() => (string)null);
            Log.Write(() => (DifferentTestObjectWithToString)null);
            Log.Write(() => (Exception)null);

            using (var log = Log.GetUsingBlockLogger(() => (TestObjectWithToString)null))
            {
                log.Write((Func<string>)null);
                log.Write(() => (string)null);
                log.Write(() => (DifferentTestObjectWithToString)null);
                log.Write(() => (Exception)null);
            }

            // Assert
            logWriter.LoggedItems.Count.Should().Be(9);
            logWriter.LoggedItems.Count(_ => _.Subject.Summary == LogHelper.NullSubjectSummary).Should().Be(4);
            logWriter.LoggedItems.Count(_ => _.Subject.Summary == Invariant($"{nameof(ActivityCorrelationPosition.First)}: {LogHelper.NullSubjectSummary}")).Should().Be(1);
            logWriter.LoggedItems.Count(_ => _.Subject.Summary == Invariant($"{nameof(ActivityCorrelationPosition.Last)}: {LogHelper.NullSubjectSummary}")).Should().Be(1);
            var middleItems = logWriter.LoggedItems.Where(_ => _.Subject.Summary == Invariant($"{nameof(ActivityCorrelationPosition.Middle)}: {LogHelper.NullSubjectSummary}")).ToList();
            middleItems.Count.Should().Be(4);
            middleItems.Count(_ => _.Context.CallingMethod != null).Should().Be(3, "We have this on everything except the null lambda.");
            middleItems.Count(_ => _.Correlations.Any(c => c is ExceptionIdCorrelation)).Should().Be(0, "Can't know if null is an exception or not.");
        }

        private class TestObjectWithToString
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test { get; set; } = Guid.NewGuid().ToString().ToUpperInvariant();

            public override string ToString()
            {
                return "HAHAHAHAH";
            }
        }

        private class DifferentTestObjectWithToString
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test { get; set; } = Guid.NewGuid().ToString().ToUpperInvariant();

            public override string ToString()
            {
                return "hjhjhjhjhjhjhjhjhj";
            }
        }

        private static InMemoryLogWriter BuildAndConfigureMemoryLogWriter()
        {
            if (memoryLogWriter == null)
            {
                var memoryLogConfig = new InMemoryLogConfig(new Dictionary<LogItemKind, IReadOnlyCollection<string>>());
                memoryLogWriter = new InMemoryLogWriter(memoryLogConfig);
                var settings = new LogWritingSettings();
                LogWriting.Instance.Setup(settings, configuredAndManagedLogWriters: new[] { memoryLogWriter }, errorCodeKeys: new[] { Constants.ExceptionDataKeyForErrorCode });
            }
            else
            {
                memoryLogWriter.PurgeAllLoggedItems();
            }

            return memoryLogWriter;
        }
    }
}
