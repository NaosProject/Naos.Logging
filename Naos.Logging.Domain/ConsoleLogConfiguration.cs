// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogConfiguration.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Globalization;

    using OBeautifulCode.Enum.Recipes;
    using OBeautifulCode.Math.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// <see cref="Console" /> focused implementation of <see cref="LogConfigurationBase" />.
    /// </summary>
    public class ConsoleLogConfiguration : LogConfigurationBase, IEquatable<ConsoleLogConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogConfiguration"/> class.
        /// </summary>
        /// <param name="contextsToLogConsoleOut"><see cref="LogContexts" /> to write to <see cref="Console.Out" />.</param>
        /// <param name="contextsToLogConsoleError"><see cref="LogContexts" /> to write to <see cref="Console.Error" />.</param>
        public ConsoleLogConfiguration(LogContexts contextsToLogConsoleOut, LogContexts contextsToLogConsoleError)
            : base(contextsToLogConsoleOut | contextsToLogConsoleError)
        {
            this.ContextsToLogConsoleOut = contextsToLogConsoleOut;
            this.ContextsToLogConsoleError = contextsToLogConsoleError;
        }

        /// <summary>
        /// Gets the <see cref="LogContexts" /> to write to <see cref="Console.Out" />.
        /// </summary>
        public LogContexts ContextsToLogConsoleOut { get; private set; }

        /// <summary>
        /// Gets the <see cref="LogContexts" /> to write to <see cref="Console.Error" />.
        /// </summary>
        public LogContexts ContextsToLogConsoleError { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(ConsoleLogConfiguration first, ConsoleLogConfiguration second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.ContextsToLog == second.ContextsToLog && first.ContextsToLogConsoleOut == second.ContextsToLogConsoleOut && first.ContextsToLogConsoleError == second.ContextsToLogConsoleError;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(ConsoleLogConfiguration first, ConsoleLogConfiguration second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(ConsoleLogConfiguration other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as ConsoleLogConfiguration);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.ContextsToLog.ToString()).Hash(this.ContextsToLogConsoleOut.ToString()).Hash(this.ContextsToLogConsoleError.ToString()).Value;
    }

    /// <summary>
    /// <see cref="Console"/> focused implementation of <see cref="LogProcessorBase" />.
    /// </summary>
    public class ConsoleLogProcessor : LogProcessorBase
    {
        private readonly ConsoleLogConfiguration consoleConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogProcessor"/> class.
        /// </summary>
        /// <param name="consoleConfiguration">Configuration.</param>
        public ConsoleLogProcessor(ConsoleLogConfiguration consoleConfiguration)
            : base(consoleConfiguration)
        {
            new { consoleConfiguration }.Must().NotBeNull().OrThrowFirstFailure();

            this.consoleConfiguration = consoleConfiguration;
        }

        /// <inheritdoc cref="LogProcessorBase" />
        public override void Log(LogMessage logMessage)
        {
            new { logMessage }.Must().NotBeNull().OrThrowFirstFailure();

            var message = Invariant($"{logMessage.LoggedDateTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logMessage.Context}|{logMessage.Message}");

            if (this.consoleConfiguration.ContextsToLogConsoleOut.HasFlagOverlap(logMessage.Context)
                && !this.consoleConfiguration.ContextsToLogConsoleOut.HasFlag(LogContexts.None))
            {
                Console.Out.WriteLine(message);
            }

            if (this.consoleConfiguration.ContextsToLogConsoleError.HasFlagOverlap(logMessage.Context)
                && !this.consoleConfiguration.ContextsToLogConsoleError.HasFlag(LogContexts.None))
            {
                Console.Error.WriteLine(message);
            }
        }

        /// <inheritdoc cref="LogProcessorBase" />
        protected override void InternalLog(LogItem logItem)
        {
            new { logItem }.Must().NotBeNull().OrThrowFirstFailure();

            var logMessage = new LogMessage(logItem.Context, logItem.BuildLogMessage(), logItem.LoggedTimeUtc);

            this.Log(logMessage);
        }

        /// <inheritdoc cref="object" />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.consoleConfiguration.ContextsToLogConsoleOut)}: {this.consoleConfiguration.ContextsToLogConsoleOut}; {nameof(this.consoleConfiguration.ContextsToLogConsoleError)}: {this.consoleConfiguration.ContextsToLogConsoleError}");
            return ret;
        }
    }
}