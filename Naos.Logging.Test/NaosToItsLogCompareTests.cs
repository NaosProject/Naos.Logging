// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosToItsLogCompareTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using OBeautifulCode.Exception.Recipes;
    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    public class NaosToItsLogCompareTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public NaosToItsLogCompareTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test_NaosLogWrite_Versus_ItsLogWrite()
        {
            // Arrange
            var memoryLogConfig =
                new InMemoryLogConfig(new Dictionary<LogItemKind, IReadOnlyCollection<string>>());
            var memoryLogWriter = new InMemoryLogWriter(memoryLogConfig);
            var settings = new LogWritingSettings();
            LogWriting.Instance.Setup(
                settings,
                configuredAndManagedLogWriters: new[] { memoryLogWriter },
                errorCodeKeys: new[] { ErrorCodeConstants.ExceptionDataKeyForErrorCode },
                multipleCallsToSetupStrategy: MultipleCallsToSetupStrategy.Overwrite);

            var itsLogStopwatch = new Stopwatch();
            var naosLogStopwatch = new Stopwatch();
            var testSubjects = Enumerable.Range(0, 100).Select(_ => Invariant($"{Guid.NewGuid().ToString()}{_}")).ToList();

            // Act
            itsLogStopwatch.Start();
            foreach (var testSubject in testSubjects)
            {
                Its.Log.Instrumentation.Log.Write(() => testSubject);
                using (var logger = Its.Log.Instrumentation.Log.Enter(() => testSubject))
                {
                    logger.Trace(() => testSubject);
                }
            }

            while (memoryLogWriter.LoggedItems.Count < testSubjects.Count * 4)
            {
                Thread.Sleep(100);
            }

            itsLogStopwatch.Stop();
            this.testOutputHelper.WriteLine("Its Log Took: " + itsLogStopwatch.Elapsed);

            memoryLogWriter.PurgeAllLoggedItems();
            naosLogStopwatch.Start();
            foreach (var testSubject in testSubjects)
            {
                Naos.Logging.Domain.Log.Write(() => testSubject);
                using (var logger = Naos.Logging.Domain.Log.With(() => testSubject))
                {
                    logger.Write(() => testSubject);
                }
            }

            while (memoryLogWriter.LoggedItems.Count < testSubjects.Count * 4)
            {
                Thread.Sleep(100);
            }

            naosLogStopwatch.Stop();
            this.testOutputHelper.WriteLine("Naos Log Took: " + naosLogStopwatch.Elapsed);

            naosLogStopwatch.Elapsed.Should().BeLessThan(itsLogStopwatch.Elapsed);

            foreach (var logItem in memoryLogWriter.LoggedItems)
            {
                var message = LogWriterBase.BuildLogMessageFromLogItem(logItem, LogItemPropertiesToIncludeInLogMessage.Default);
                this.testOutputHelper.WriteLine(message);
            }
        }
    }
}
