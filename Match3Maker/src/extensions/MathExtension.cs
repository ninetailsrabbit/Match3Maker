namespace SystemExtensions {
    public static class MathExtension {
        /// <summary>
        /// Checks if an integer value falls between a specified minimum and maximum range.
        /// </summary>
        /// <param name="value">The integer value to check.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <param name="inclusive">Optional flag indicating whether the range includes the minimum and maximum values (default: true).</param>
        /// <returns>True if the value is between min and max (inclusive or exclusive based on the flag), False otherwise.</returns>
        public static bool IsBetween(this int value, int min, int max, bool inclusive = true) {
            int minValue = Math.Min(min, max);
            int maxValue = Math.Max(min, max);

            return inclusive ? value >= minValue && value <= maxValue : value > minValue && value < maxValue;
        }

        /// <summary>
        /// Checks if a float value falls between a specified minimum and maximum range, considering a small precision offset.
        /// </summary>
        /// <param name="value">The float value to check.</param>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <param name="inclusive">Optional flag indicating whether the range includes the minimum and maximum values (default: true).</param>
        /// <param name="precision">Optional precision value to account for floating-point rounding errors (default: 0.00001f).</param>
        /// <returns>True if the value is between min and max (inclusive or exclusive based on the flag), False otherwise.</returns>
        public static bool IsBetween(this float value, float min, float max, bool inclusive = true, float precision = 0.00001f) {
            float minValue = Math.Min(min, max) - precision;
            float maxValue = Math.Max(min, max) + precision;

            return inclusive ? value >= minValue && value <= maxValue : value > minValue && value < maxValue;
        }
    }
}
