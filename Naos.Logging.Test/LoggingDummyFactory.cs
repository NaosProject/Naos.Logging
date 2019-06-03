// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingDummyFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Recipes
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using FakeItEasy;
    using Naos.Logging;
    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using Naos.Serialization.Domain;
    using OBeautifulCode.AutoFakeItEasy;

    /// <summary>
    /// A dummy factory for Accounting Time types.
    /// </summary>
#if !NaosLoggingRecipesProject
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("Naos.Logging", "See package version number")]
#endif
    public class LoggingDummyFactory : IDummyFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingDummyFactory"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is not excessively complex.  Dummy factories typically wire-up many types.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is not excessively complex.  Dummy factories typically wire-up many types.")]
        public LoggingDummyFactory()
        {
            var logWriterConfigTypes = typeof(LogWriting).Assembly.DefinedTypes.Where(_ => _.BaseType == typeof(LogWriterConfigBase)).ToList();
            AutoFixtureBackedDummyFactory.UseRandomConcreteSubclassForDummy<LogWriterConfigBase>(
                null,
                logWriterConfigTypes);

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var result = new LogItemContext(A.Dummy<DateTime>().ToUniversalTime(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

                    return result;
                });


            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var context = A.Dummy<LogItemContext>();
                    var dummySubject = new DummySubject { Info = A.Dummy<string>() };
                    var result = new LogItem(new RawSubject(dummySubject, dummySubject.Info).ToSubject(), LogItemKind.Object, context, A.Dummy<string>());

                    return result;
                });



            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () =>
                {
                    var result = new RawSubject(A.Dummy<string>(), A.Dummy<string>());

                    return result;
                });
        }

        /// <inheritdoc />
        public Priority Priority => new FakeItEasy.Priority(1);

        /// <inheritdoc />
        public bool CanCreate(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        public object Create(Type type)
        {
            return null;
        }
    }

    /// <summary>
    /// Used when making a dummy <see cref="LogItem" />.
    /// </summary>
    public class DummySubject
    {
        public string Info { get; set; }
    }
}
