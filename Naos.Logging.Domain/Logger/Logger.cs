// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Representation;
    using static System.FormattableString;

    /// <summary>
    /// Default implementation of <see cref="ILog" />.  Messages will be sent to a configured <see cref="LogItemHandler" /> which can be configured directly or via LogWriting.Setup in Persistence.
    /// </summary>
    public class Logger : ILog
    {
        private readonly string defaultComment;

        private readonly string defaultOrigin;

        private readonly IManageCorrelations correlationManager;

        private readonly object syncSetHandler = new object();

        private LogItemHandler logItemHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logItemHandler">Optional <see cref="LogItemHandler" /> to process <see cref="LogItem" />'s; DEFAULT is a null handler.</param>
        /// <param name="correlationManager">Optional correlation manager potentially with active correlations; DEFAULT is a new <see cref="CorrelationManager" />.</param>
        /// <param name="defaultOrigin">Optional default origin to use; DEFAULT is <see cref="LogItemOrigin.NaosLoggingLogger" />.</param>
        /// <param name="defaultComment">Optional default comment to use; DEFAULT is null.</param>
        public Logger(LogItemHandler logItemHandler = null, IManageCorrelations correlationManager = null, string defaultOrigin = null, string defaultComment = null)
        {
            this.defaultComment = defaultComment;
            this.defaultOrigin = defaultOrigin ?? LogItemOrigin.NaosLoggingLogger.ToString();
            this.logItemHandler = logItemHandler ?? DefaultLogItemHandler;
            this.correlationManager = correlationManager ?? new CorrelationManager();
        }

        private static void DefaultLogItemHandler(LogItem logItem)
        {
            /* no-op */
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Write(Func<object> subjectFunc, string comment = null, string origin = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            LogItem logItem = null;
            try
            {
                logItem = LogHelper.BuildLogItem(this.correlationManager, subjectFunc, comment ?? this.defaultComment, origin ?? this.defaultOrigin, additionalCorrelations: additionalCorrelations);
            }
            catch (Exception failedToBuildException)
            {
                LastDitchLogger.LogError(Invariant($"Error in {this.GetType().ToStringReadable()}.{nameof(this.Write)} - {nameof(failedToBuildException)}: {failedToBuildException}"));
            }

            try
            {
                this.logItemHandler(logItem);
            }
            catch (Exception failedToWriteException)
            {
                LastDitchLogger.LogError(Invariant($"Error in {this.GetType().ToStringReadable()}.{nameof(this.Write)} - {nameof(failedToWriteException)}: {failedToWriteException}"));
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Preference is lowercase guids.")]
        public ILogDisposable With(Func<object> correlatingSubjectFunc = null, string defaultCommentOverride = null, string defaultOriginOverride = null, string correlationId = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            var nestedCorrelationManager = this.correlationManager.ShallowClone();

            var localCorrelationId = correlationId ?? Guid.NewGuid().ToString().ToLowerInvariant();
            if (correlatingSubjectFunc != null)
            {
                nestedCorrelationManager.AddSubjectCorrelation(correlatingSubjectFunc, localCorrelationId);
            }

            nestedCorrelationManager.AddOrderCorrelation(correlationId: localCorrelationId);
            nestedCorrelationManager.AddElapsedCorrelation(localCorrelationId);
            nestedCorrelationManager.PrepareExceptionCorrelations(correlationId: localCorrelationId);
            nestedCorrelationManager.AddAdditionalCorrelations(additionalCorrelations);

            var result = new UsingBlockLogger(
                nestedCorrelationManager,
                this.logItemHandler,
                defaultOriginOverride ?? this.defaultOrigin,
                defaultCommentOverride ?? this.defaultComment);

            return result;
        }

        /// <inheritdoc />
        public void SetCallback(LogItemHandler callback)
        {
            lock (this.syncSetHandler)
            {
                this.logItemHandler = callback;
            }
        }
    }
}