// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsingBlockLogger.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Correlating logger that will log traces to a specific activity that can be grouped later with a shared subject, timings, and position.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Unnecessary here.")]
    public class UsingBlockLogger : Logger, ILogDisposable
    {
        /// <summary>
        /// Initial item subject.
        /// </summary>
        public const string InitialItemOfUsingBlockSubject = "Initial Item of Using Block.";

        /// <summary>
        /// Final item subject;
        /// </summary>
        public const string FinalItemOfUsingBlockSubject = "Final Item of Using Block.";

        /// <summary>
        /// Initializes a new instance of the <see cref="UsingBlockLogger"/> class.
        /// </summary>
        /// <param name="correlationManager">Optional correlation manager potentially with active correlations; DEFAULT is a new <see cref="CorrelationManagerr" />.</param>
        /// <param name="logItemHandler">Handler callback to log send generated messages to.</param>
        /// <param name="origin">Origin override.</param>
        /// <param name="initialComment">Comment for first and last message.</param>
        public UsingBlockLogger(
            IManageCorrelations correlationManager,
            LogItemHandler logItemHandler,
            string origin,
            string initialComment)
            : base(logItemHandler, correlationManager, origin, initialComment)
        {
            this.Write(() => InitialItemOfUsingBlockSubject);
            
            //should we post subject here???
            //write initial message here? include correlationid(s)?
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", Justification = "Unnecessary here.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Unnecessary here.")]
        public void Dispose()
        {
            this.Write(() => FinalItemOfUsingBlockSubject);

            //write final message here? signal is final, put something in subject??
        }
    }
}