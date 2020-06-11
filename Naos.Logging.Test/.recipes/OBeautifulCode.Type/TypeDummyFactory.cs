﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDummyFactory.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Type.Test source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Type.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.Math.Recipes;

    /// <summary>
    /// A Dummy Factory for types in <see cref="OBeautifulCode.Type"/>.
    /// </summary>
#if !OBeautifulCodeTypeRecipesProject
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Type.Test", "See package version number")]
#endif
    public class TypeDummyFactory : DefaultTypeDummyFactory
    {
        private static readonly IReadOnlyList<Type> LoadedTypes =
            AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a => (a != null) && (!a.IsDynamic))
                .SelectMany(_ => _.GetTypes())
                .Where(_ => _ != null)
                .Where(_ => !string.IsNullOrWhiteSpace(_.Namespace))
                .ToList();

        private static readonly IReadOnlyList<Type> AppDomainClosedTypes =
            LoadedTypes
                .Where(_ => !_.ContainsGenericParameters)
                .ToList();

        private static readonly IReadOnlyList<Type> AppDomainGenericTypeDefinitions =
            LoadedTypes
                .Where(_ => _.IsGenericTypeDefinition)
                .ToList();

        public TypeDummyFactory()
        {
            AutoFixtureBackedDummyFactory.AddDummyCreator(() =>
            {
                var startDateTime = A.Dummy<DateTime>();

                var endDateTime = A.Dummy<DateTime>().ThatIs(_ => _ >= startDateTime);

                var result = new UtcDateTimeRangeInclusive(startDateTime.ToUniversalTime(), endDateTime.ToUniversalTime());

                return result;
            });

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    Type result;

                    if (ThreadSafeRandom.Next(0, 2) == 0)
                    {
                        result = GetRandomClosedTypeInAppDomain();
                    }
                    else
                    {
                        while (true)
                        {
                            var genericTypeDefinition = GetRandomGenericTypeDefinitionInAppDomain();

                            var typeArguments = Enumerable.Range(0, genericTypeDefinition.GetGenericArguments().Length).Select(_ => GetRandomClosedTypeInAppDomain()).ToArray();

                            try
                            {
                                result = genericTypeDefinition.MakeGenericType(typeArguments);

                                break;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }

                    return result;
                });
        }

        private Type GetRandomClosedTypeInAppDomain()
        {
            var randomIndex = ThreadSafeRandom.Next(0, AppDomainClosedTypes.Count);

            var result = AppDomainClosedTypes[randomIndex];

            return result;
        }

        private Type GetRandomGenericTypeDefinitionInAppDomain()
        {
            var randomIndex = ThreadSafeRandom.Next(0, AppDomainGenericTypeDefinitions.Count);

            var result = AppDomainGenericTypeDefinitions[randomIndex];

            return result;
        }
    }
}