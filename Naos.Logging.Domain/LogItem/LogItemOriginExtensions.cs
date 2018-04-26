// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemOriginExtensions.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    using System;

    /// <summary>
    /// Extensions methods on type <see cref="LogItemOrigin"/>.
    /// </summary>
    public static class LogItemOriginExtensions
    {
        /// <summary>
        /// Converts a <see cref="LogItemOrigin"/> to a <see cref="LogItemOrigins"/>.
        /// </summary>
        /// <param name="origin">The log-item origin.</param>
        /// <returns>
        /// A <see cref="LogItemOrigins"/> converted from a <see cref="LogItemOrigin"/>.
        /// </returns>
        public static LogItemOrigins ToOrigins(
            this LogItemOrigin origin)
        {
            var result = (LogItemOrigins)Enum.Parse(typeof(LogItemOrigins), origin.ToString());
            return result;
        }
    }
}
