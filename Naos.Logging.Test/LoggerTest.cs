﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;

    using Naos.Logging.Domain;

    using Xunit;
    using Xunit.Abstractions;
    using static System.FormattableString;

    public class LoggerTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public LoggerTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ILog", Justification = "Spelling/name is correct.")]
        [Fact]
        public void Log_and_ILog__Have_same_methods()
        {
            // Arrange
            var staticClassType = typeof(Log);
            var interfaceLogType = typeof(ILog);

            var methodNamesOnClassToIgnore = new[]
            {
                nameof(Log.ToString),
                nameof(Log.GetType),
                nameof(Log.GetHashCode),
                nameof(Log.Equals),
                nameof(Log.SetupSubjectSerialization),
                Invariant($"get_{nameof(Log.SubjectSerializerRepresentation)}"),
                Invariant($"get_{nameof(Log.SubjectSerializerFactory)}"),
                Invariant($"get_{nameof(Log.Instance)}"),
            }.ToList();

            // Act
            var staticLogMethods = staticClassType.GetMethods().Where(_ => !methodNamesOnClassToIgnore.Contains(_.Name)).ToList();
            var interfaceLogMethods = interfaceLogType.GetMethods().ToList();

            string BuildCompareNameFromMethod(MethodInfo info)
            {
                var parameterString = string.Join(
                    ",",
                    info.GetParameters().Select(_ => Invariant($"ParamName:{_.Name}|ParamType:{_.ParameterType}")));

                var result = Invariant($"MethodName:{info.Name}-Parameters:{parameterString}");
                return result;
            }

            var staticLogMethodDescriptors = staticLogMethods.Select(BuildCompareNameFromMethod).ToList();
            var interfaceLogMethodDescriptors = interfaceLogMethods.Select(BuildCompareNameFromMethod).ToList();

            this.testOutputHelper.WriteLine(nameof(Log));
            staticLogMethodDescriptors.ForEach(this.testOutputHelper.WriteLine);
            this.testOutputHelper.WriteLine(string.Empty);
            this.testOutputHelper.WriteLine(nameof(ILog));
            interfaceLogMethodDescriptors.ForEach(this.testOutputHelper.WriteLine);

            // Assert
            staticLogMethods.Count.Should().Be(interfaceLogMethods.Count);
            staticLogMethodDescriptors.Intersect(interfaceLogMethodDescriptors).Count().Should().Be(staticLogMethods.Count);
        }

        [Fact]
        public void NestedUsing___Correctly_logged_to___Includes_all_correlations()
        {
            var items = new List<LogItem>();
            Log.SetCallback(_ => items.Add(_));

            using (var topLogger = Log.With(() => "Top."))
            {
                topLogger.Write(() => "Message in top before middle.");

                using (var middleLogger = topLogger.With(() => "Middle."))
                {
                    middleLogger.Write(() => "Message in middle before bottom.");

                    using (var bottomLogger = middleLogger.With(() => "Bottom."))
                    {
                        bottomLogger.Write(() => "Message in bottom.");
                    }

                    middleLogger.Write(() => "Message in middle after bottom.");
                }

                topLogger.Write(() => "Message in top after middle.");
            }

            items.Select(_ => Invariant($"SubjectSummary: {_.ToString()}; Correlations: {string.Join(",", _.Correlations.OrderBy(c => c.CorrelationId).Select(c => c.ToString()))}")).ToList().ForEach(this.testOutputHelper.WriteLine);
        }
    }
}
