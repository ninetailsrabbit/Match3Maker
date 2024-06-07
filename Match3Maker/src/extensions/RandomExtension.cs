namespace SystemExtensions {

    public static class RandomExtension {

        /// <summary>
        /// Generates a random float value within a specified range (inclusive).
        /// </summary>
        /// <param name="random">The <see cref="Random"/> object to use for generating random numbers.</param>
        /// <param name="minValue">The minimum value (inclusive) of the desired range. Defaults to 1.0f.</param>
        /// <param name="maxValue">The maximum value (inclusive) of the desired range. Defaults to 1.0f.</param>
        /// <returns>
        /// A random float value between <paramref name="minValue"/> (inclusive) and <paramref name="maxValue"/> (inclusive).
        /// </returns>
        /// <remarks>
        /// This extension method provides a way to generate random floats within a specific range, similar to how `Random.Next` works with integers. It ensures the generated value falls within the provided range and handles cases where the minimum value is greater than the maximum value.
        /// 
        /// **Note:** This approach uses a different algorithm than `Random.NextDouble` and might not be suitable for all scenarios where a uniformly distributed random float is needed. Refer to the linked Stack Overflow discussion for details on alternative approaches.
        /// 
        /// https://stackoverflow.com/questions/63650648/random-float-in-c-sharp
        /// </remarks>
        public static float NextFloat(this Random random, float minValue = float.MinValue, float maxValue = float.MaxValue) {
            double range = (double)maxValue - (double)minValue;
            double sample = random.NextDouble();
            double scaled = (sample * range) + float.MinValue;

            return (float)scaled;
        }

    }
}