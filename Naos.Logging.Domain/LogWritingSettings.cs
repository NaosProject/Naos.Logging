// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogWritingSettings.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System.Collections.Generic;

    /// <summary>
    /// Settings to use when configuring log writing.
    /// </summary>
    public class LogWritingSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogWritingSettings"/> class.
        /// </summary>
        /// <param name="configs">Configurations to setup; DEFAULT is null but only to be used in null logging scenarios or custom provided log writers.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "configs", Justification = "Spelling/name is correct.")]
        public LogWritingSettings(
            IReadOnlyCollection<LogWriterConfigBase> configs = null)
        {
            this.Configs = configs ?? new List<LogWriterConfigBase>();
        }

        /// <summary>
        /// Gets the configurations to use.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configs", Justification = "Spelling/name is correct.")]
        public IReadOnlyCollection<LogWriterConfigBase> Configs { get; private set; }
    }
}