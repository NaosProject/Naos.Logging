// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemKind.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Determines the kind of log-item.
    /// </summary>
    public enum LogItemKind
    {
        /// <summary>
        /// Unknown (default).
        /// </summary>
        Unknown,

        /// <summary>
        /// An informational log-item.
        /// </summary>
        Info,

        /// <summary>
        /// An error occured and is being logged.
        /// </summary>
        Error,
    }
}
