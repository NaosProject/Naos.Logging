// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Default implementation of <see cref="ILog" />.  Messages will be sent to a configured <see cref="LoggerCallback" /> which can be configured directly or via LogWriting.Setup in Persistence.
    /// </summary>
    public class Logger : ILog
    {
        private LoggerCallback logItemHandler = DefaultLogItemHandler;

        private static void DefaultLogItemHandler(LogItem logItem)
        {
            /* no-op */
        }

        /// <inheritdoc />
        public virtual void Write(Func<object> subjectFunc, string comment = null, string originOverride = null)
        {
            var logItem = LogHelper.BuildLogItem(subjectFunc, comment, originOverride ?? LogItemOrigin.NaosLoggingLogger.ToString());
            this.logItemHandler(logItem);
        }

        /// <inheritdoc />
        public virtual void Write(string subject, string comment = null, string originOverride = null)
        {
            this.Write(() => subject, comment, originOverride);
        }

        /// <inheritdoc />
        public virtual ICorrelatingActivityLogger Enter(Func<object> correlatingSubjectFunc, string comment = null, string originOverride = null)
        {
            var result = new CorrelatingActivityLogger(correlatingSubjectFunc, comment, originOverride ?? LogItemOrigin.NaosLoggingLogger.ToString(), this.logItemHandler);

            return result;
        }

        /// <inheritdoc />
        public void SetCallback(LoggerCallback callback)
        {
            this.logItemHandler = callback;
        }
    }
}