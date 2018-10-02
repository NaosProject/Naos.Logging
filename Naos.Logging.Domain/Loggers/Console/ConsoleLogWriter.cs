// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    using static System.FormattableString;

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

            var logMessage = BuildLogMessageFromLogItem(logItem, this.consoleConfig.LogItemPropertiesToIncludeInLogMessage, true);

            if (this.consoleConfig.ShouldLogConsole(logItem.Kind, logItem.Context.Origin))
            {
                Console.Out.WriteLine(logMessage);
            }

            if (this.consoleConfig.ShouldLogError(logItem.Kind, logItem.Context.Origin))
            {
                Console.Error.WriteLine(logMessage);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var ret = Invariant($"{this.GetType().FullName}; {nameof(this.consoleConfig.LogInclusionKindToOriginsMapForConsoleOut)}: {this.consoleConfig.LogInclusionKindToOriginsMapForConsoleOutFriendlyString}; {nameof(this.consoleConfig.LogInclusionKindToOriginsMapForConsoleError)}: {this.consoleConfig.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString}");
            return ret;
        }
    }
}