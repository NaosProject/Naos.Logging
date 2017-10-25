// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogProcessorSettings.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Settings to use when setup log processing.
    /// </summary>
    public class LogProcessorSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogProcessorSettings"/> class.
        /// </summary>
        /// <param name="configurations">Configurations to setup.</param>
        public LogProcessorSettings(IReadOnlyCollection<LogConfigurationBase> configurations)
        {
            this.Configurations = configurations ?? new List<LogConfigurationBase>();
        }

        /// <summary>
        /// Gets the configurations to use.
        /// </summary>
        public IReadOnlyCollection<LogConfigurationBase> Configurations { get; private set; }
    }
}