// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWritingForNaosLoggerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using OBeautifulCode.Exception.Recipes;
    using OBeautifulCode.Serialization.Json;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    public class LogWritingForNaosLoggerTest
    {
        private readonly string expectedOrigin = LogItemOrigin.NaosLoggingLogger.ToString();
        private readonly List<LogItem> loggedItems = new List<LogItem>();
        private readonly ITestOutputHelper testOutputHelper;

        public LogWritingForNaosLoggerTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            Log.SetCallback(this.SaveLogItems);
        }

        private void SaveLogItems(LogItem logItem)
        {
            this.loggedItems.Add(logItem);
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
                logName: "NaosLoggingTestLogName",
                source: "NaosLoggingTestSource");
            var json = new ObcJsonSerializer().SerializeToString(config);
            this.testOutputHelper.WriteLine(json);
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_string_subject_via_lambda_without_comment()
        {
            // Arrange
            var subject = "this is a string subject " + A.Dummy<string>();

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<string>();

            logItem.Subject.Summary.Should().Be(subject);
            actualSubject.Should().Be(subject);
            logItem.Kind.Should().Be(LogItemKind.String);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_real_object_subject_via_lambda_with_comment()
        {
            // Arrange
            var subject = new TestObjectWithToString();
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<TestObjectWithToString>();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            actualSubject.ToString().Should().Be(subject.ToString());
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_real_object_subject_via_lambda_without_comment()
        {
            // Arrange
            var subject = new TestObjectWithToString();

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<TestObjectWithToString>();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            actualSubject.ToString().Should().Be(subject.ToString());
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_anonymous_object_subject_via_lambda_with_comment()
        {
            // Arrange
            var subject = new { Message = "hello" };
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = this.loggedItems.Single();
            dynamic actualSubject = logItem.Subject.DeserializeSubject();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            ((string)actualSubject.Message).Should().Be(subject.Message);
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_anonymous_object_subject_via_lambda_without_comment()
        {
            // Arrange
            var subject = new { Message = "hello" };

            // Act
            Log.Write(() => subject);

            // Assert
            var logItem = this.loggedItems.Single();
            dynamic actualSubject = logItem.Subject.DeserializeSubject();

            logItem.Subject.Summary.Should().Be(subject.ToString());
            ((string)actualSubject.Message).Should().Be(subject.Message);
            logItem.Kind.Should().Be(LogItemKind.Object);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [Fact]
        public void LogWrite___Should_create_LogItem___When_logging_string_subject_via_lambda_with_comment()
        {
            // Arrange
            var subject = "this is a string subject " + A.Dummy<string>();
            var comment = "this is a comment " + A.Dummy<string>();

            // Act
            Log.Write(() => subject, comment);

            // Assert
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<string>();

            logItem.Subject.Summary.Should().Be(subject);
            actualSubject.Should().Be(subject);
            logItem.Kind.Should().Be(LogItemKind.String);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().BeNull();
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.Should().NotBeNull();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            logItem.Correlations.Should().BeEmpty();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid external failures.")]
        [Fact]
        public void Log_Write___Records_details_correctly___With_exception_and_no_comment()
        {
            // Arrange
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
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().BeNull();

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single();
            exceptionCorrelation.ExceptionId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid external failures.")]
        [Fact]
        public void Log_Write___Records_details_correctly___With_exception_and_comment()
        {
            // Arrange
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
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single();
            exceptionCorrelation.ExceptionId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid external failures.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need lowercase.")]
        [Fact]
        public void Log_Write___Records_details_correctly___With_exception_and_comment_and_error_code()
        {
            // Arrange
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
            var logItem = this.loggedItems.Single();
            var actualSubject = logItem.Subject.DeserializeSubject<InvalidOperationException>();

            logItem.Subject.Summary.Should().Be(Invariant($"{nameof(InvalidOperationException)}: {exceptionMessage}"));
            actualSubject.Message.Should().Be(exceptionMessage);

            logItem.Kind.Should().Be(LogItemKind.Exception);
            logItem.Comment.Should().Be(comment);

            logItem.Context.Should().NotBeNull();
            logItem.Context.StackTrace.Should().Be(actualSubject.StackTrace);
            logItem.Context.Origin.Should().Be(this.expectedOrigin);
            logItem.Context.CallingMethod.Should().NotBeNullOrWhiteSpace();
            logItem.Context.CallingType.AssemblyQualifiedName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.MachineName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessFileVersion.Should().NotBeNullOrWhiteSpace();
            logItem.Context.ProcessName.Should().NotBeNullOrWhiteSpace();
            logItem.Context.TimestampUtc.Should().BeOnOrBefore(DateTime.UtcNow);

            var errorCodeCorrelation = (ErrorCodeCorrelation)logItem.Correlations.Single(_ => _.GetType() == typeof(ErrorCodeCorrelation));
            errorCodeCorrelation.ErrorCode.Should().Be(errorCode);

            var exceptionCorrelation = (ExceptionIdCorrelation)logItem.Correlations.Single(_ => _.GetType() == typeof(ExceptionIdCorrelation));
            exceptionCorrelation.ExceptionId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid external failures.")]
        [Fact]
        public void Log_Write___Records_details_correctly___With_exception_and_inner_exception()
        {
            // Arrange
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
            var timeOrderedItems = this.loggedItems.OrderBy(_ => _.Context.TimestampUtc).ToList();
            timeOrderedItems.Count.Should().Be(2);
            var logItemInner = this.loggedItems.First();
            var logItem = this.loggedItems.Last();

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

            innerCorrelation.ExceptionId.Should().Be(innerSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());
            exceptionCorrelation.ExceptionId.Should().Be(actualSubject.GetExceptionIdFromExceptionData(searchInnerExceptionChain: true).ToString());

            innerCorrelation.ExceptionId.Should().Be(exceptionCorrelation.ExceptionId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching to avoid external failures.")]
        [Fact]
        public void Log_Enter_Activity_Trace___Records_details_correctly___With_exception_and_comment()
        {
            // Arrange
            var enterSubject = new TestObjectWithToString();
            var stringTraceWithLambda = "some trace with a lambda" + A.Dummy<string>();
            var traceObjectWithLambda = new DifferentTestObjectWithToString();
            var exceptionToThrow = new InvalidOperationException("Oh no.");

            // Act
            using (var log = Log.With(() => enterSubject))
            {
                // to make sure the next message doesn't come too quickly for the assert later.
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
            this.loggedItems.ToList().ForEach(_ => _.Correlations.Single(c => c is ElapsedCorrelation).Should().NotBeNull());
            this.loggedItems.ToList().ForEach(_ => _.Correlations.Single(c => c is SubjectCorrelation).Should().NotBeNull());
            this.loggedItems.Select(_ => _.Correlations.Single(c => c is ElapsedCorrelation)).Count().Should()
                .Be(this.loggedItems.Count);
            this.loggedItems.Select(_ => _.Correlations.Single(c => c is SubjectCorrelation)).Count().Should()
                .Be(this.loggedItems.Count);
            this.loggedItems.ToList().ForEach(
                _ => ((SubjectCorrelation)_.Correlations.Single(c => c is SubjectCorrelation)).Subject
                    .DeserializeSubject<TestObjectWithToString>().Test.Should().Be(enterSubject.Test));

            var enterItem = this.loggedItems.Single(_ => _.Correlations.Any(c => c is OrderCorrelation oc && oc.Position == 0));
            enterItem.Subject.DeserializeSubject<string>().Should().Be(UsingBlockLogger.InitialItemOfUsingBlockSubject);
            var enterCorrelation = (ElapsedCorrelation)enterItem.Correlations.Single(_ => _ is ElapsedCorrelation);
            enterCorrelation.ElapsedTime.TotalMilliseconds.Should().Be(0);

            var middleItems = this.loggedItems.Where(
                _ => new[] { exceptionToThrow.Message, stringTraceWithLambda, traceObjectWithLambda.ToString() }.Any(
                    a => _.Subject.Summary.EndsWith(a, StringComparison.CurrentCulture))).ToList();
            middleItems.ForEach(_ =>
            {
                _.Correlations.Any(c => c is OrderCorrelation oc && oc.Position > 0 && oc.Position < 4).Should().BeTrue();
                var middleCorrelation = (ElapsedCorrelation)_.Correlations.Single(s => s is ElapsedCorrelation);
                middleCorrelation.ElapsedTime.TotalMilliseconds.Should().BeGreaterThan(0);
            });

            var exitItem = this.loggedItems.Single(_ => _.Correlations.Any(c => c is OrderCorrelation oc && oc.Position == 4));
            exitItem.Subject.DeserializeSubject<string>().Should().Be(UsingBlockLogger.FinalItemOfUsingBlockSubject);
            var exitCorrelation = (ElapsedCorrelation)exitItem.Correlations.Single(_ => _ is ElapsedCorrelation);
            exitCorrelation.ElapsedTime.TotalMilliseconds.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Log_Write___MultipleNestedLoggers_should_accumulate_correlations()
        {
            // Arrange
            var firstCorrelationId = Guid.NewGuid().ToString();
            var firstLoggerSubject = "First Subject";

            var secondCorrelationId = Guid.NewGuid().ToString();
            var secondLoggerSubject = "Second Subject";
            var secondCorrelatingSubject = "Second Correlating Subject";

            var thirdCorrelationId = Guid.NewGuid().ToString();
            var thirdLoggerSubject = "Third Subject";
            var thirdCorrelatingSubject = "Third Correlating Subject";

            // Act
            using (var first = Log.With(correlationId: firstCorrelationId))
            {
                first.Write(() => firstLoggerSubject);
                using (var second = first.With(() => secondCorrelatingSubject, correlationId: secondCorrelationId))
                {
                    second.Write(() => secondLoggerSubject);
                    using (var third = second.With(() => thirdCorrelatingSubject, correlationId: thirdCorrelationId))
                    {
                        third.Write(() => thirdLoggerSubject);
                    }
                }
            }

            // Assert
            this.loggedItems.Count.Should().Be(9);
            this.loggedItems.SelectMany(_ => _.Correlations.Select(c => c.CorrelationId).Distinct()).Count(_ => _ == firstCorrelationId).Should().Be(9);
            this.loggedItems.SelectMany(_ => _.Correlations.Select(c => c.CorrelationId).Distinct()).Count(_ => _ == secondCorrelationId).Should().Be(6);
            this.loggedItems.SelectMany(_ => _.Correlations.Select(c => c.CorrelationId).Distinct()).Count(_ => _ == thirdCorrelationId).Should().Be(3);

            foreach (var logItem in this.loggedItems)
            {
                logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == firstCorrelationId && _ is ElapsedCorrelation).Should().NotBeNull("All items should have first elapsed time correlation");
                logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == firstCorrelationId && _ is OrderCorrelation).Should().NotBeNull("All items should have first order correlation");

                if (logItem.Correlations.Select(c => c.CorrelationId).Contains(secondCorrelationId))
                {
                    var subjectCorrelation = logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == secondCorrelationId && _ is SubjectCorrelation) as SubjectCorrelation;
                    subjectCorrelation.Should().NotBeNull("All items should have first elapsed time correlation");
                    (subjectCorrelation?.Subject.DeserializeSubject<string>() ?? throw new InvalidOperationException("Should have already asserted is not null.")).Should().Be(secondCorrelatingSubject);
                    logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == secondCorrelationId && _ is ElapsedCorrelation).Should().NotBeNull();
                    logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == secondCorrelationId && _ is OrderCorrelation).Should().NotBeNull();
                }

                if (logItem.Correlations.Select(c => c.CorrelationId).Contains(thirdCorrelationId))
                {
                    var subjectCorrelation = logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == thirdCorrelationId && _ is SubjectCorrelation) as SubjectCorrelation;
                    subjectCorrelation.Should().NotBeNull("All items should have first elapsed time correlation");
                    (subjectCorrelation?.Subject.DeserializeSubject<string>() ?? throw new InvalidOperationException("Should have already asserted is not null.")).Should().Be(thirdCorrelatingSubject);
                    logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == thirdCorrelationId && _ is ElapsedCorrelation).Should().NotBeNull();
                    logItem.Correlations.SingleOrDefault(_ => _.CorrelationId == thirdCorrelationId && _ is OrderCorrelation).Should().NotBeNull();
                }
            }
        }

        [Fact]
        public void Log_Write___Should_make_null_subject_LogItem___When_nulls_written()
        {
            // Arrange & Act
            Log.Write(() => (string)null);
            Log.Write(() => (DifferentTestObjectWithToString)null);
            Log.Write(() => (Exception)null);

            using (var log = Log.With(() => (TestObjectWithToString)null))
            {
                log.Write((Func<string>)null);
                log.Write(() => (string)null);
                log.Write(() => (DifferentTestObjectWithToString)null);
                log.Write(() => (Exception)null);
            }

            // Assert
            this.loggedItems.Count.Should().Be(9);
            this.loggedItems.Count(_ => _.Subject.Summary == LogHelper.NullSubjectSummary && _.Correlations.Count == 0).Should().Be(3);
            this.loggedItems.Count(_ => _.Subject.Summary == UsingBlockLogger.InitialItemOfUsingBlockSubject).Should().Be(1);
            this.loggedItems.Count(_ => _.Subject.Summary == UsingBlockLogger.FinalItemOfUsingBlockSubject).Should().Be(1);
            var middleItems = this.loggedItems.Where(_ => _.Subject.Summary == LogHelper.NullSubjectSummary && _.Correlations.Any(c => c is OrderCorrelation oc && oc.Position > 0 && oc.Position < 5)).ToList();
            middleItems.Count.Should().Be(4);
            middleItems.Count(_ => _.Context.CallingMethod != null).Should().Be(3, "We have this on everything except the null lambda.");
            middleItems.Count(_ => _.Correlations.Any(c => c is ExceptionIdCorrelation)).Should().Be(0, "Can't know if null is an exception or not.");
        }

        private class TestObjectWithToString
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test { get; set; } = Guid.NewGuid().ToString().ToLowerInvariant();

            public override string ToString()
            {
                return "HAHAHAHAH";
            }
        }

        private class DifferentTestObjectWithToString
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test { get; set; } = Guid.NewGuid().ToString().ToLowerInvariant();

            public override string ToString()
            {
                return "hjhjhjhjhjhjhjhjhj";
            }
        }
    }
}
