// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerationTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;

    using Naos.Logging.Domain;

    using Xunit;

    using static System.FormattableString;

    public static class LoggerTest
    {
        [Fact]
        public static void Log_and_ILog__Have_same_methods()
        {
            // Arrange
            var staticClassType = typeof(Log);
            var interfaceLogType = typeof(ILog);

            var classSkipNames = new[]
            {
                nameof(Log.ToString),
                nameof(Log.GetType),
                nameof(Log.GetHashCode),
                nameof(Log.Equals),
                Invariant($"get_{nameof(Log.Instance)}"),
            }.ToList();

            // Act
            var staticLogMethods = staticClassType.GetMethods().Where(_ => !classSkipNames.Contains(_.Name)).ToList();
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

            // Assert
            staticLogMethods.Count.Should().Be(interfaceLogMethods.Count);
            staticLogMethodDescriptors.Intersect(interfaceLogMethodDescriptors).Count().Should().Be(staticLogMethods.Count);
        }
    }
}
