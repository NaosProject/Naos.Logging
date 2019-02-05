// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerCallback.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Domain
{
    /// <summary>
    /// Callback to wire up processing of items logged to <see cref="Log" />.
    /// </summary>
    /// <param name="logItem">Log item logged.</param>
    public delegate void LoggerCallback(LogItem logItem);
}