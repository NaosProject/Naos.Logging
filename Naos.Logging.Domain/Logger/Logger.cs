﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Collections.Generic;

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
        /// <param name="correlationManager">Optional correlation manager potentially with active correlations; DEFAULT is a new <see cref="CorrelationManagerr" />.</param>
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
        public void Write(Func<object> subjectFunc, string comment = null, string origin = null, IReadOnlyCollection<IHaveCorrelationId> additionalCorrelations = null)
        {
            // what do we pass to build log items to build correlations?  just the manager?  do we foreach correlations we havent seen b/c error doesnt work that way?
            try
            {
                var logItem = LogHelper.BuildLogItem(this.correlationManager, subjectFunc, comment ?? this.defaultComment, origin ?? this.defaultOrigin, additionalCorrelations: additionalCorrelations);
                this.logItemHandler(logItem);
            }
            catch (Exception e)
            {
                // what do we log? should we tostring the subject? should we use a more permissive serializer?
                // could have failed to build log item issue...
                LastDitchLogger.LogError("");
            }
        }

        /// <inheritdoc />
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