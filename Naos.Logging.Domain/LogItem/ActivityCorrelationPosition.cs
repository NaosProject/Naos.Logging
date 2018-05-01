// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityCorrelationPosition.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Determines the position of a log-item within a defined
    /// block of code that explicitly relates those items.
    /// </summary>
    public enum ActivityCorrelationPosition
    {
        /// <summary>
        /// Unknown (default).
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents the first log-item in a timeseries of correlated log-items.
        /// </summary>
        First,

        /// <summary>
        /// Represents somewhere in the middle of a timeseries of correlated log-items.
        /// </summary>
        Middle,

        /// <summary>
        /// Represents the last log-item in a timeseries of correlated log-items.
        /// </summary>
        Last,
    }
}
