// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleCallsToSetupStrategy.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Enumeration of options when a setup method is called more than once.
    /// </summary>
    public enum MultipleCallsToSetupStrategy
    {
        /// <summary>
        /// Invalid default state.
        /// </summary>
        Invalid,

        /// <summary>
        /// Throw an exception.
        /// </summary>
        Throw,

        /// <summary>
        /// Configure with new settings.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Ignore the new settings.
        /// </summary>
        Ignore,
    }
}