#region License

// Copyright (c) K2 Workflow (SourceCode Technology Holdings Inc.). All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

#endregion

namespace SourceCode.Clay.Json.Validation
{
    public sealed class DoubleValidator
    {
        #region Properties

        public double? Min { get; }

        public double? Max { get; }

        public bool MinExclusive { get; }

        public bool MaxExclusive { get; }

        public bool Required { get; }

        public long? MultipleOf { get; }

        #endregion

        #region Constructors

        public DoubleValidator(double? min, double? max, bool minExclusive, bool maxExclusive, bool required, long? multipleOf)
        {
            Min = min;
            if (Min.HasValue && minExclusive)
                MinExclusive = true;

            Max = max;
            if (Max.HasValue && maxExclusive)
                MaxExclusive = true;

            Required = required;
            MultipleOf = multipleOf;
        }

        public DoubleValidator(double? min, double? max, bool required)
            : this(min, max, false, false, required, null)
        { }

        public DoubleValidator(double? min, double? max)
            : this(min, max, false, false, false, null)
        { }

        #endregion

        #region Methods

        public bool IsValid(double? value)
        {
            // Check Required
            if (!value.HasValue)
                return !(Required); // null + optional = true, null + required = false

            // Check Min
            if (Min.HasValue)
            {
                if (MinExclusive)
                {
                    if (value.Value <= Min.Value) return false;
                }
                else if (value.Value < Min.Value) return false;
            }

            // Check Max
            if (Max.HasValue)
            {
                if (MaxExclusive)
                {
                    if (value.Value >= Max.Value) return false;
                }
                else if (value.Value > Max.Value) return false;
            }

            // MultipleOf
            if (MultipleOf.HasValue
                && MultipleOf.Value != 0 // n % 0 == undefined
                && value.Value != 0.0) // 0 % n == 0 (we already know value.HasValue is true)
            {
                var zero = value.Value % MultipleOf.Value == 0.0; // Modulus(Double) is a well-defined operation
                if (!zero) return false;
            }

            return true;
        }

        public override string ToString()
        {
            // Use set notation for open/closed boundaries

            // Required
            var s = (Required ? "Required: " : string.Empty)

            // Min
            + (MinExclusive ? "(" : "[")
            + (Min.HasValue ? $"{Min.Value}" : "-∞")
            + ", "

            // Max
            + (Max.HasValue ? $"{Max.Value}" : "∞")
            + (MaxExclusive ? ")" : "]")

            // MultipleOf
            + (MultipleOf.HasValue && MultipleOf.Value != 0 ? $" (x{MultipleOf.Value})" : string.Empty);

            return s;
        }

        #endregion
    }
}
