// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CorrelatingActivityLogger.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;
    using System.Diagnostics;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Correlating logger that will log traces to a specific activity that can be grouped later with a shared subject, timings, and position.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Unnecessary here.")]
    public class CorrelatingActivityLogger : ICorrelatingActivityLogger
    {
        private readonly string initialComment;
        private readonly string origin;
        private readonly LoggerCallback logItemHandler;
        private readonly Stopwatch stopwatch;
        private readonly string correlationId;
        private readonly object correlatingSubject;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelatingActivityLogger"/> class.
        /// </summary>
        /// <param name="correlatingSubjectFunc">Function to get the correlating subject.</param>
        /// <param name="initialComment">Comment for first and last message.</param>
        /// <param name="origin">Origin override.</param>
        /// <param name="logItemHandler">Handler callback to log send generated messages to.</param>
        public CorrelatingActivityLogger(Func<object> correlatingSubjectFunc, string initialComment, string origin, LoggerCallback logItemHandler)
        {
            new { correlatingSubjectFunc }.Must().NotBeNull();
            new { logItemHandler }.Must().NotBeNull();

            this.initialComment = initialComment;
            this.origin = origin;
            this.logItemHandler = logItemHandler;

            this.stopwatch = Stopwatch.StartNew();
            this.correlatingSubject = correlatingSubjectFunc();
            this.correlationId = this.correlatingSubject?.GetHashCode().ToGuid().ToString().ToUpperInvariant() ?? Guid.NewGuid().ToString().ToUpperInvariant();
            var activityCorrelationPosition = ActivityCorrelationPosition.First;
            var summary = LogHelper.BuildSummaryFromSubjectObject(this.correlatingSubject, activityCorrelationPosition);
            var correlatingPreppedSubject = new RawSubject(this.correlatingSubject, summary).ToSubject();
            var activityCorrelation = new ActivityCorrelation(this.correlationId, correlatingPreppedSubject, 0, activityCorrelationPosition);

            var initialLogItem = LogHelper.BuildLogItem(
                () => this.correlatingSubject,
                this.initialComment,
                this.origin,
                activityCorrelation);

            this.logItemHandler(initialLogItem);
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "Unnecessary here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Unnecessary here.")]
        public void Dispose()
        {
            this.stopwatch.Stop();
            var activityCorrelationPosition = ActivityCorrelationPosition.Last;
            var summary = LogHelper.BuildSummaryFromSubjectObject(this.correlatingSubject, activityCorrelationPosition);
            var correlatingPreppedSubject = new RawSubject(this.correlatingSubject, summary).ToSubject();
            var activityCorrelation = new ActivityCorrelation(this.correlationId, correlatingPreppedSubject, this.stopwatch.ElapsedMilliseconds, activityCorrelationPosition);

            var finalLogItem = LogHelper.BuildLogItem(
                () => this.correlatingSubject,
                this.initialComment,
                this.origin,
                activityCorrelation);

            this.logItemHandler(finalLogItem);
        }

        /// <inheritdoc />
        public void Trace(Func<object> subjectFunc, string comment = null, string originOverride = null)
        {
            var activityCorrelationPosition = ActivityCorrelationPosition.Middle;
            var summary = LogHelper.BuildSummaryFromSubjectObject(this.correlatingSubject, activityCorrelationPosition);
            var correlatingPreppedSubject = new RawSubject(this.correlatingSubject, summary).ToSubject();
            var activityCorrelation = new ActivityCorrelation(this.correlationId, correlatingPreppedSubject, this.stopwatch.ElapsedMilliseconds, activityCorrelationPosition);

            var middleLogItem = LogHelper.BuildLogItem(
                subjectFunc,
                comment,
                originOverride ?? this.origin,
                activityCorrelation);

            this.logItemHandler(middleLogItem);
        }

        /// <inheritdoc />
        public void Trace(string subject, string comment = null, string originOverride = null)
        {
            this.Trace(() => subject, comment, originOverride);
        }
    }
}