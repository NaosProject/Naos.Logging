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
        /// <param name="configurations">Configurations to setup; DEFAULT is null but only to be used in null logging scenarios or custom provided log processors.</param>
        public LogProcessorSettings(IReadOnlyCollection<LogWriterConfigBase> configurations = null)
        {
            this.Configurations = configurations ?? new List<LogWriterConfigBase>();
        }

        /// <summary>
        /// Gets the configurations to use.
        /// </summary>
        public IReadOnlyCollection<LogWriterConfigBase> Configurations { get; private set; }
    }
}