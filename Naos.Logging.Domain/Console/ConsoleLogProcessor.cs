// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogProcessor.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Globalization;

    using OBeautifulCode.Enum.Recipes;

    using Spritely.Recipes;

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

            var message = FormattableString.Invariant($"{logMessage.LoggedDateTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logMessage.Context}|{logMessage.Message}");

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
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.consoleConfiguration.ContextsToLogConsoleOut)}: {this.consoleConfiguration.ContextsToLogConsoleOut}; {nameof(this.consoleConfiguration.ContextsToLogConsoleError)}: {this.consoleConfiguration.ContextsToLogConsoleError}");
            return ret;
        }
    }
}