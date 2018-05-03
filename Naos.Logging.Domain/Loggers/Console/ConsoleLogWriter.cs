// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using OBeautifulCode.Enum.Recipes;

    /// <summary>
    /// <see cref="Console"/> focused implementation of <see cref="LogWriterBase" />.
    /// </summary>
    public class ConsoleLogWriter : LogWriterBase
    {
        private readonly ConsoleLogConfig consoleConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogWriter"/> class.
        /// </summary>
        /// <param name="consoleConfig">Configuration.</param>
        public ConsoleLogWriter(
            ConsoleLogConfig consoleConfig)
            : base(consoleConfig)
        {
            this.consoleConfig = consoleConfig ?? throw new ArgumentNullException(nameof(consoleConfig));
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            if (logItem == null)
            {
                throw new ArgumentNullException(nameof(logItem));
            }

            var logMessage = BuildLogMessageFromLogEntry(logItem, this.consoleConfig.LogItemPropertiesToIncludeInLogMessage, true);

            var origins = logItem.Context.Origin.ToOrigins();
            if (this.consoleConfig.OriginsToLogConsoleOut.HasFlagOverlap(origins))
            {
                Console.Out.WriteLine(logMessage);
            }

            if (this.consoleConfig.OriginsToLogConsoleError.HasFlagOverlap(origins))
            {
                Console.Error.WriteLine(logMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = FormattableString.Invariant($"{this.GetType().FullName}; {nameof(this.consoleConfig.OriginsToLogConsoleOut)}: {this.consoleConfig.OriginsToLogConsoleOut}; {nameof(this.consoleConfig.OriginsToLogConsoleError)}: {this.consoleConfig.OriginsToLogConsoleError}");
            return ret;
        }
    }
}