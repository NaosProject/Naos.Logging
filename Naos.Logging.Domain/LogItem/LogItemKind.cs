﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Determines the kind of log-item based around it's content.
    /// </summary>
    public enum LogItemKind
    {
        /// <summary>
        /// Unknown (default)
        /// </summary>
        Unknown,

        /// <summary>
        /// A string is the content.
        /// </summary>
        String,

        /// <summary>
        /// An object is the content.
        /// </summary>
        Object,

        /// <summary>
        /// A derivative of <see cref="System.Exception" /> is the content.
        /// </summary>
        Exception,

        /// <summary>
        /// An implementation of Naos.Telemetry.Domain.IAmTelemetryItem is the content.
        /// </summary>
        Telemetry,
    }
}
