#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System.Json;

namespace SourceCode.Clay.OpenApi.Serialization
{
    /// <summary>
    /// Represents a serializer that can convert between OpenAPI objects and JSON.
    /// </summary>
    public interface IOpenApiSerializer
    {
        #region Methods

        /// <summary>
        /// Serializes the specified OpenAPI object to a JSON value.
        /// </summary>
        /// <typeparam name="T">The type of the OpenAPI object.</typeparam>
        /// <param name="value">The instance of the OpenAPI object.</param>
        /// <returns>The serialized value.</returns>
        JsonValue Serialize<T>(T value);

        /// <summary>
        /// Deserializes the specified OpenAPI object from a JSON value.
        /// </summary>
        /// <typeparam name="T">The expected type of the OpenAPI object.</typeparam>
        /// <param name="value">The JSON value to deserialize.</param>
        /// <returns>The OpenAPI object.</returns>
        T Deserialize<T>(JsonValue value);

        #endregion
    }
}
