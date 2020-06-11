﻿// --------------------------------------------------------------------------------------------------------------------
// <auto-generated>
//   Generated using OBeautifulCode.CodeGen.ModelObject (1.0.79.0)
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Representation.System.Test
{
    using global::System;
    using global::System.CodeDom.Compiler;
    using global::System.Collections.Concurrent;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Linq.Expressions;

    using global::FakeItEasy;

    using global::OBeautifulCode.AutoFakeItEasy;
    using global::OBeautifulCode.Representation.System;

    /// <summary>
    /// The default (code generated) Dummy Factory.
    /// Derive from this class to add any overriding or custom registrations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [GeneratedCode("OBeautifulCode.CodeGen.ModelObject", "1.0.79.0")]
    public abstract class DefaultRepresentationSystemDummyFactory : IDummyFactory
    {
        public DefaultRepresentationSystemDummyFactory()
        {
            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new AssemblyRepresentation(
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ElementInitRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<MethodInfoRepresentation>(),
                                 A.Dummy<IReadOnlyList<ExpressionRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberAssignmentRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<MemberInfoRepresentation>(),
                                 A.Dummy<ExpressionRepresentationBase>()));


            AutoFixtureBackedDummyFactory.UseRandomConcreteSubclassForDummy<MemberBindingRepresentationBase>();

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberListBindingRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<MemberInfoRepresentation>(),
                                 A.Dummy<IReadOnlyList<ElementInitRepresentation>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberMemberBindingRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<MemberInfoRepresentation>(),
                                 A.Dummy<IReadOnlyCollection<MemberBindingRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new BinaryExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionType>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<ExpressionRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ConditionalExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionType>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<ExpressionRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ConstructorInfoRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberInfoRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MethodInfoRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<string>(),
                                 A.Dummy<IReadOnlyList<TypeRepresentation>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new UnknownTypePlaceholder());


            AutoFixtureBackedDummyFactory.UseRandomConcreteSubclassForDummy<ExpressionRepresentationBase>();

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new InvocationExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<IReadOnlyList<ExpressionRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new LambdaExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<IReadOnlyList<ParameterExpressionRepresentation>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ListInitExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<NewExpressionRepresentation>(),
                                 A.Dummy<IReadOnlyList<ElementInitRepresentation>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<MemberInfoRepresentation>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MemberInitExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<NewExpressionRepresentation>(),
                                 A.Dummy<IReadOnlyCollection<MemberBindingRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new MethodCallExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionType>(),
                                 A.Dummy<ExpressionRepresentationBase>(),
                                 A.Dummy<MethodInfoRepresentation>(),
                                 A.Dummy<IReadOnlyList<ExpressionRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new NewArrayExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionType>(),
                                 A.Dummy<IReadOnlyList<ExpressionRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new NewExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ConstructorInfoRepresentation>(),
                                 A.Dummy<IReadOnlyList<ExpressionRepresentationBase>>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new ParameterExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<string>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TypeBinaryExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new UnaryExpressionRepresentation(
                                 A.Dummy<TypeRepresentation>(),
                                 A.Dummy<ExpressionType>(),
                                 A.Dummy<ExpressionRepresentationBase>()));

            AutoFixtureBackedDummyFactory.AddDummyCreator(
                () => new TypeRepresentation(
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<string>(),
                                 A.Dummy<IReadOnlyList<TypeRepresentation>>()));
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
}