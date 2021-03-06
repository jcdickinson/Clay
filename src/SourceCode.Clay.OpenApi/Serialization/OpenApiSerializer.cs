#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Json;

namespace SourceCode.Clay.OpenApi.Serialization
{
    /// <summary>
    /// Represents a serializer that can convert between OpenAPI objects and JSON.
    /// </summary>
    public partial class OpenApiSerializer : IOpenApiSerializer
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiSerializer"/> class.
        /// </summary>
        public OpenApiSerializer()
        { }

        #endregion

        #region Deserialize

        /// <summary>Deserializes the specified OpenAPI object from a JSON value.</summary>
        /// <typeparam name="T">The expected type of the OpenAPI object.</typeparam>
        /// <param name="value">The JSON value to deserialize.</param>
        /// <returns>The OpenAPI object.</returns>
        public T Deserialize<T>(JsonValue value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
