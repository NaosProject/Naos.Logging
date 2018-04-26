// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs" company="Naos">
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
            new { consoleConfig }.Must().NotBeNull().OrThrowFirstFailure();

            this.consoleConfig = consoleConfig;
        }

        /// <inheritdoc />
        protected override void LogInternal(
            LogItem logItem)
        {
            var message = FormattableString.Invariant($"{logItem.Context.LoggedTimeUtc.ToString("o", CultureInfo.InvariantCulture)}|{logItem.Context}|{logItem.Message}");

            var origins = logItem.Context.LogItemOrigin.ToOrigins();
            if (this.consoleConfig.OriginsToLogConsoleOut.HasFlagOverlap(origins))
            {
                Console.Out.WriteLine(message);
            }

            if (this.consoleConfig.OriginsToLogConsoleError.HasFlagOverlap(origins))
            {
                Console.Error.WriteLine(message);
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