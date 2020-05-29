// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Logging.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Logging.Domain;
    using Naos.Logging.Persistence;
    using OBeautifulCode.Serialization.Bson;
    using OBeautifulCode.Serialization.Json;
    using Xunit;

    using static System.FormattableString;

    public static class SerializationTests
    {
        private static readonly ObcBsonSerializer BsonSerializer = new ObcBsonSerializer(typeof(LoggingBsonSerializationConfiguration).ToBsonSerializationConfigurationType());
        private static readonly ObcJsonSerializer JsonSerializer = new ObcJsonSerializer(typeof(LoggingJsonSerializationConfiguration).ToJsonSerializationConfigurationType());

        [Fact]
        public static void RawSubject_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<RawSubject>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<RawSubject>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<RawSubject>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void ElapsedCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<ElapsedCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<ElapsedCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<ElapsedCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void ErrorCodeCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<ErrorCodeCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<ErrorCodeCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<ErrorCodeCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void ExceptionIdCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<ExceptionIdCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<ExceptionIdCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<ExceptionIdCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void IdCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<IdCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<IdCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<IdCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void LogItem_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<LogItem>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<LogItem>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<LogItem>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void LogItemContext_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<LogItemContext>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<LogItemContext>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<LogItemContext>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void OrderCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<OrderCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<OrderCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<OrderCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void Subject_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<Subject>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<Subject>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<Subject>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }

        [Fact]
        public static void SubjectCorrelation_Roundtrips()
        {
            // Arrange
            var expected = A.Dummy<SubjectCorrelation>();

            // Act
            var actualBsonString = BsonSerializer.SerializeToString(expected);
            var actualBson = BsonSerializer.Deserialize<SubjectCorrelation>(actualBsonString);

            var actualJsonString = JsonSerializer.SerializeToString(expected);
            var actualJson = JsonSerializer.Deserialize<SubjectCorrelation>(actualJsonString);

            // Assert
            actualBson.Should().Be(expected);
            actualJson.Should().Be(expected);
        }
    }
}
