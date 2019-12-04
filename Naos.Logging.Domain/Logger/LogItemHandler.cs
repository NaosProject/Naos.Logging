// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogItemHandler.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Callback to wire up processing of items logged to <see cref="Log" />.
    /// </summary>
    /// <param name="logItem">Log item logged.</param>
    public delegate void LogItemHandler(LogItem logItem);
}
