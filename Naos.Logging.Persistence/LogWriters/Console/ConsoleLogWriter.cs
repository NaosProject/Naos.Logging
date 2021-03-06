// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogWriter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Persistence
{
    using System;
    using Naos.Logging.Domain;
    using OBeautifulCode.Representation.System;
    using OBeautifulCode.Serialization;
    using OBeautifulCode.Type.Recipes;
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
        /// <param name="logItemSerializerFactory">Optional serializer factory; DEFAULT will be base version.</param>
        public ConsoleLogWriter(
            ConsoleLogConfig consoleConfig,
            ISerializerFactory logItemSerializerFactory = null)
            : base(consoleConfig, logItemSerializerFactory)
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

            var logMessage = BuildLogMessageFromLogItem(logItem, this.consoleConfig.LogItemPropertiesToIncludeInLogMessage, this.BuildSerializer(), true);

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
            var ret = Invariant($"{this.GetType().ToStringReadable()}; {nameof(this.consoleConfig.LogInclusionKindToOriginsMapForConsoleOut)}: {this.consoleConfig.LogInclusionKindToOriginsMapForConsoleOutFriendlyString}; {nameof(this.consoleConfig.LogInclusionKindToOriginsMapForConsoleError)}: {this.consoleConfig.LogInclusionKindToOriginsMapForConsoleErrorFriendlyString}");
            return ret;
        }
    }
}
