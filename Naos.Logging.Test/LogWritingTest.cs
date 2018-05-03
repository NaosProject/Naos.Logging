// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWritingTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Its.Log.Instrumentation;
    using Naos.Compression.Domain;
    using Naos.Logging.Domain;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.Json;
    using Xunit;

    public static class LogWritingTest
    {
        private static InMemoryLogWriter memoryLogWriter;

        [Fact]
        public static void LogWrite___Should_create_LogItem___When_logging_string_subject_without_comment()
        {
            // Arrange
            var memoryLogWriter = BuildAndConfigureMemoryLogWriter();
            var logItemSubject = "this is a string subject";
            var logItemSubjectDescribedSerialization = logItemSubject.ToDescribedSerializationUsingSpecificFactory(LogWriting.SubjectSerializationDescription, JsonSerializerFactory.Instance, CompressorFactory.Instance);

            // Act
            Log.Write(logItemSubject);

            // Assert
            var actual = memoryLogWriter.LoggedItems.Single();
            actual.Comment.Should().BeNull();
            actual.Context.Should().NotBeNull();
            actual.Context.Origin.Should().Be(LogItemOrigin.ItsLogEntryPostedInformation);
            actual.Correlations.Should().BeEmpty();
            actual.Kind.Should().Be(LogItemKind.Info);
            actual.Subject.DescribedSerialization.Should().Be(logItemSubjectDescribedSerialization);
        }

        [Fact]
        public static void Test()
        {
            try
            {
                TestMethod();
            }
            catch (Exception ex)
            {
                Log.Write(() => ex);
            }

            Log.Write(() => "I made a mistake", "comment");
            Log.Write(() => new MyTestObj2());
            Log.Write(new MyTestObj());
            Log.Write(() => new MyTestObj(), "some comment");
            Log.Write(new MyTestObj(), "some comment");
            Log.Write(() => new { Whatever = "whatever", TestObj = new MyTestObj(), TestObj2 = new MyTestObj2() });
            Log.Write(new { Whatever = "whatever" });

            using (var log = Log.Enter(() => new { ContextItem1 = "some context" }))
            {
                log.Trace(() => new InvalidOperationException("some exception occurred"));
                log.Trace("some trace");
                log.Trace(() => "some trace 2");
                log.Trace(() => new MyTestObj2());
                using (var log2 = Log.Enter(() => new { ContextItem2 = "some context" }))
                {
                    log2.Trace("some trace");
                }
            }
        }

        private static void TestMethod()
        {
            throw new InvalidOperationException("BAD!!");
        }

        private class MyTestObj
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test => "abcd";

            public override string ToString()
            {
                return "HAHAHAHAH";
            }
        }

        private class MyTestObj2
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For testing.")]
            public string Test => "abcd";
        }

        private static InMemoryLogWriter BuildAndConfigureMemoryLogWriter()
        {
            if (memoryLogWriter == null)
            {
                var memoryLogConfig = new InMemoryLogConfig(LogItemOrigins.All, LogItemPropertiesToIncludeInLogMessage.SubjectSummary);
                memoryLogWriter = new InMemoryLogWriter(memoryLogConfig);
                var settings = new LogWritingSettings();
                LogWriting.Instance.Setup(settings, configuredAndManagedLogWriters: new[] { memoryLogWriter });
            }
            else
            {
                memoryLogWriter.PurgeAllLoggedItems();
            }

            return memoryLogWriter;
        }
    }
}
