// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHaveCorrelationId.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Identifies an object that has a correlation identifier.
    /// </summary>
    public interface IHaveCorrelationId
    {
        /// <summary>
        /// Gets an identifier used to correlate multiple log-items.
        /// </summary>
        string CorrelationId { get; }
    }
}
