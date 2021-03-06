#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SourceCode.Clay.Collections.Generic
{
    /// <summary>
    /// Base class for dynamic switches.
    /// </summary>
    /// <typeparam name="TKey">The type of keys.</typeparam>
    /// <typeparam name="TValue">The type of values.</typeparam>
    internal sealed class SwitchBuilderImpl<TKey, TValue> : IDynamicSwitch<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
    {
        #region Fields

        private readonly IReadOnlyList<TValue> _values;
        private readonly Func<TKey, int> _indexer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchBuilderImpl{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="cases">The cases.</param>
        public SwitchBuilderImpl(IReadOnlyDictionary<TKey, TValue> cases)
        {
            var (values, indexer) = BuildSwitchExpression(cases);

            _values = values;
            _indexer = indexer;
        }

        #endregion

        #region IDynamicSwitch

        /// <summary>
        /// The number of items in the switch.
        /// </summary>
        public int Count => _values.Count;

        /// <summary>
        /// Gets the value with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="TValue"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key] => _values[_indexer(key)];

        /// <summary>
        /// Checks whether the specified key is present in the switch.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) => _indexer(key) >= 0;

        /// <summary>
        /// Attempts to get the value corresponding to the specified key.
        /// </summary>
        /// <param name="key">The key value.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            var ix = _indexer(key);

            if (ix < 0) return false;

            value = _values[ix];
            return true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Builds the underlying <see cref="Expression"/> based switch.
        /// </summary>
        /// <param name="cases">The cases to transform into a dynamic switch.</param>
        /// <returns>A lambda that returns an index for a specified key value.</returns>
        private (TValue[] ar, Func<TKey, int> func) BuildSwitchExpression(IReadOnlyDictionary<TKey, TValue> cases)
        {
            TValue[] values;
            Expression<Func<TKey, int>> expr;

            // Return -1 if key is not found (per standard convention for IndexOf())
            var notFound = Expression.Constant(-1);

            // Define formal parameter
            var formalParam = Expression.Parameter(typeof(TKey), "key");

            // Fast path if no cases
            if (cases == null || cases.Count == 0)
            {
                values = Array.Empty<TValue>();
                expr = Expression.Lambda<Func<TKey, int>>(notFound, formalParam);
            }
            else
            {
                values = new TValue[cases.Count];
                var unique = new Dictionary<TKey, int>(cases.Count, EqualityComparer<TKey>.Default);

                // Extract values and ensure keys are unique
                var i = 0;
                foreach (var @case in cases)
                {
                    values[i] = @case.Value;

                    // Rely on this throwing if there are any duplicates
                    unique.Add(@case.Key, i);

                    i++;
                }

                // Create <Key, SwitchCase>[] list
                i = 0;
                var switchCases = new SwitchCase[cases.Count];
                foreach (var @case in unique)
                {
                    // Create Case Expression
                    var key = Expression.Constant(@case.Key);
                    var value = Expression.Constant(@case.Value);

                    switchCases[i] = Expression.SwitchCase(value, key);

                    i++;
                }

                // Create Switch Expression
                var switchExpr = Expression.Switch(formalParam, notFound, switchCases);

                // Create final Expression
                expr = Expression.Lambda<Func<TKey, int>>(switchExpr, formalParam);
            }

            // Compile Expression
            var func = expr.Compile();
            return (values, func);
        }

        #endregion
    }
}
