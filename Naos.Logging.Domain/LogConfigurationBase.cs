// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.ComponentModel;

    /// <summary>
    /// Base class for all log configuration.
    /// </summary>
    [Bindable(BindableSupport.Default)]
    public abstract class LogConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogConfigurationBase"/> class.
        /// </summary>
        /// <param name="contextsToLog">Contexts to log.</param>
        protected LogConfigurationBase(LogContexts contextsToLog)
        {
            this.ContextsToLog = contextsToLog;
        }

        /// <summary>
        /// Gets the contexts to log from.
        /// </summary>
        public LogContexts ContextsToLog { get; private set; }
    }
}