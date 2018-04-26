// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MachineName.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Diagnostics.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.NetworkInformation;

    /// <summary>
    /// Uses various methods to get the name of a machine.
    /// </summary>
#if !NaosDiagnosticsDomain
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.Diagnostics", "See package version number")]
#endif
    internal static class MachineName
    {
        /// <summary>
        /// Gets the name of this machine, using various methods to come
        /// up with a genrally "good" name for use in a broad set of scenarios.
        /// </summary>
        /// <returns>
        /// The name of this machine.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "The caller will be the consumer of this recipe.")]
        public static string GetMachineName()
        {
            var result = GetResolvedLocalhostName();
            if (result == "localhost")
            {
                result = GetFullyQualifiedDomainName();
                if (string.IsNullOrEmpty(result))
                {
                    result = GetNetBiosName();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the name of this machine using various methods of determining the name.
        /// </summary>
        /// <returns>
        /// A map of the kind of machine name to the machine name.
        /// </returns>
        public static IReadOnlyDictionary<MachineNameKind, string> GetMachineNames()
        {
            var result = new Dictionary<MachineNameKind, string>
            {
                { MachineNameKind.NetBiosName, GetNetBiosName() },
                { MachineNameKind.FullyQualifiedDomainName, GetFullyQualifiedDomainName() },
                { MachineNameKind.ResolvedLocalhostName, GetResolvedLocalhostName() },
            };

            return result;
        }

        /// <summary>
        /// Gets the NetBIOS name of this local computer.
        /// </summary>
        /// <returns>
        /// The NetBIOS name of this local computer.
        /// </returns>
        public static string GetNetBiosName()
        {
            var result = Environment.MachineName;
            return result;
        }

        /// <summary>
        /// Gets the fully qualified domain name.
        /// </summary>
        /// <returns>
        /// The fully qualified domain name.
        /// </returns>
        /// <remarks>
        /// Adapted from <a href="https://stackoverflow.com/a/804719/356790" />
        /// </remarks>
        public static string GetFullyQualifiedDomainName()
        {
            var result = Dns.GetHostName();

            var domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;

            if (!string.IsNullOrEmpty(domainName))
            {
                domainName = "." + domainName;

                // if hostname does not already include domain name
                if (!result.EndsWith(domainName, StringComparison.Ordinal))
                {
                    // add the domain name part
                    result += domainName;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the resolved localhost name.
        /// </summary>
        /// <returns>
        /// Gets the resolved
        /// </returns>
        public static string GetResolvedLocalhostName()
        {
            var result = Dns.GetHostEntry("localhost").HostName;
            return result;
        }
    }
}
