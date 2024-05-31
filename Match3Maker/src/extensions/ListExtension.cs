namespace SystemExtensions {

    public static class ListExtension {
        private static readonly Random _rng = new();

        public static int LastIndex<T>(this IEnumerable<T> sequence) => sequence.Count() - 1;

        /// <summary>
        /// Gets the middle element of an IList if it has at least 3 elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IList.</typeparam>
        /// <param name="sequence">The IList to find the middle element of.</param>
        /// <returns>The middle element of the sequence if it has at least 3 elements, throws an ArgumentException otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if the sequence has less than 3 elements.</exception>
        public static T MiddleElement<T>(this IList<T> sequence) {
            if (sequence.Count >= 3) {
                int middleIndex = sequence.Count / 2;

                return sequence[middleIndex];
            }

            throw new ArgumentException("ListExtension:MiddleElement -> Sequence must contain at least 3 elements.");
        }

        /// <summary>
        /// Removes duplicate elements from an IEnumerable sequence and returns a new list containing the unique elements.
        /// </summary>
        /// <param name="sequence">The IEnumerable sequence to remove duplicates from.</param>
        /// <returns>A new List containing the unique elements from the input sequence.</returns>
        public static IEnumerable<T> RemoveDuplicates<T>(this IEnumerable<T> sequence) => sequence.Distinct();

        /// <summary>
        /// Filters out null elements from an IEnumerable sequence and returns a new sequence containing only non-null elements.
        /// </summary>
        /// <param name="sequence">The IEnumerable sequence to remove null elements from.</param>
        /// <returns>A new IEnumerable sequence containing only non-null elements from the input sequence.</returns>
        /// <typeparam name="T">The type of elements in the sequence (assumed to be nullable).</typeparam>
        /// <exception cref="InvalidOperationException">Throws if the source sequence is null.</exception>
        public static IEnumerable<T> RemoveNullables<T>(this IEnumerable<T> sequence) => sequence.Where(x => x is not null);

        /// <summary>
        /// Checks if an IEnumerable sequence is empty (contains no elements).
        /// </summary>
        /// <param name="sequence">The IEnumerable sequence to check for emptiness.</param>
        /// <returns>True if the sequence contains no elements, False otherwise.</returns>
        /// <exception cref="ArgumentNullException">Throws an ArgumentNullException if the input sequence is null.</exception>
        public static bool IsEmpty<T>(this IEnumerable<T> sequence) {
            ArgumentNullException.ThrowIfNull(sequence);

            return !sequence.Any();
        }

        /// <summary>
        /// Shuffles the elements of the specified IList in random order using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements contained in the IList.</typeparam>
        /// <param name="sequence">The IList to be shuffled.</param>
        public static void Shuffle<T>(this IList<T> sequence) {
            int n = sequence.Count;


            while (n > 1) {
                n--;
                int k = _rng.Next(n + 1);
                (sequence[n], sequence[k]) = (sequence[k], sequence[n]);
            }
        }

        /// <summary>
        /// Calculates the average (arithmetic mean) of a sequence of integer values.
        /// </summary>
        /// <param name="numbers">The sequence of integer numbers.</param>
        /// <returns>The average value of the numbers, or 0 if the sequence is empty.</returns>
        public static int Average(this IEnumerable<int> numbers) {
            if (numbers.IsEmpty())
                return 0;

            return numbers.Aggregate(0, (accum, current) => accum + current);
        }

        /// <summary>
        /// Calculates the average (arithmetic mean) of a sequence of floating-point values.
        /// </summary>
        /// <param name="numbers">The sequence of floating-point numbers.</param>
        /// <returns>The average value of the numbers, or 0.0f if the sequence is empty.</returns>
        public static float Average(this IEnumerable<float> numbers) {
            if (numbers.IsEmpty())
                return 0.0f;

            return numbers.Aggregate(0.0f, (accum, current) => accum + current);
        }

        /// <summary>
        /// Creates a deep copy of a provided IEnumerable{T} collection, where T implements the ICloneable interface.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IEnumerable{T} collection, which must implement ICloneable.</typeparam>
        /// <param name="sequence">The IEnumerable{T} collection to clone.</param>
        /// <returns>A new IEnumerable{T} collection containing deep copies of the original elements.</returns>
        /// <remarks>
        /// This extension method provides a generic approach to cloning an IEnumerable{T} collection. It assumes that the elements within the collection implement the `ICloneable` interface. 
        /// 
        /// The method leverages LINQ's `Select` to iterate through each item in the original sequence. For each item, it calls the `Clone` method (assumed to be implemented by `T`) to create a deep copy. The resulting copies are then cast back to type `T` and added to a new List object using `ToList`.
        /// 
        /// This approach ensures a new, independent collection is created with deep copies of the original elements. Modifications to the cloned collection won't affect the original data.
        /// </remarks>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> sequence) where T : ICloneable {
            return sequence.Select(item => (T)item.Clone());
        }


        /// <summary>
        /// Creates a deep copy of a provided IList{T} collection, where T implements the ICloneable interface.
        /// </summary>
        /// <typeparam name="T">The type of elements in the IList{T} collection, which must implement ICloneable.</typeparam>
        /// <param name="sequence">The IList{T} collection to clone.</param>
        /// <returns>A new IList{T} collection containing deep copies of the original elements.</returns>
        /// <remarks>
        /// This extension method provides a generic approach to cloning an IList{T} collection. It assumes that the elements within the collection implement the `ICloneable` interface. 
        /// 
        /// The method leverages LINQ's `Select` to iterate through each item in the original sequence. For each item, it calls the `Clone` method (assumed to be implemented by `T`) to create a deep copy. The resulting copies are then cast back to type `T` and added to a new List object using `ToList`.
        /// 
        /// This approach ensures a new, independent collection is created with deep copies of the original elements. Modifications to the cloned collection won't affect the original data.
        /// </remarks>
        public static IList<T> Clone<T>(this IList<T> sequence) where T : ICloneable {
            return [.. sequence.Clone()];
        }

        /// <summary>
        /// Performs a recursive flattening operation on an IEnumerable collection.
        /// </summary>
        /// <param name="enumerable">The source IEnumerable collection to flatten. Can contain any object types.</param>
        /// <returns>An IEnumerable sequence containing elements from the flattened structure.</returns>
        /// <remarks>
        /// This function iterates through the elements of the provided `enumerable` collection. 
        /// If an element is itself an IEnumerable (like a list or another nested collection), it recursively calls `Flatten` on that element to further flatten its contents. 
        /// Otherwise, the element is directly yielded back to the caller. 
        /// 
        /// This process continues until all nested collections are flattened, resulting in a single sequence of elements from the original structure.
        /// </remarks>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumerable) {
            foreach (T element in enumerable) {
                if (element is IEnumerable<T> candidate) {
                    foreach (T nested in Flatten<T>(candidate)) {
                        yield return nested;
                    }
                }
                else {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Performs a recursive `SelectMany` operation on a sequence, applying a selector function that also returns sequences.
        /// </summary>
        /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to start the recursive selection.</param>
        /// <param name="selector">A function that selects a sequence of elements for each element in the source.</param>
        /// <returns>A flattened sequence containing all elements from the source and nested sequences selected by the selector.</returns>
        /// <exception cref="ArgumentNullException">Throws if source or selector is null.</exception>
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector) {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source.IsEmpty() ? source :
                source.Concat(
                    source
                    .SelectMany(i => selector(i).EmptyIfNull())
                    .SelectManyRecursive(selector)
                );
        }

        /// <summary>
        /// Returns an empty sequence if the source sequence is null, otherwise returns the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The sequence to check for null.</param>
        /// <returns>An empty sequence if source is null, otherwise the source sequence.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source) => source ?? [];

        /// <summary>
        /// Selects a random element from the provided sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The IEnumerable collection to select from.</param>
        /// <returns>A random element from the sequence.</returns>
        /// <remarks>
        /// This function utilizes an extension method to simplify random element selection. It delegates the actual logic to the `RandomElementUsing` function, passing a newly created `Random` instance for internal use.
        /// </remarks>
        public static T RandomElement<T>(this IEnumerable<T> sequence) => sequence.RandomElementUsing<T>(new Random());

        /// <summary>
        /// Selects a random element from the provided sequence using a specified Random instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="sequence">The IEnumerable collection to select from.</param>
        /// <param name="rand">The Random instance to use for generating randomness (optional, new Random() used by default in RandomElement).</param>
        /// <returns>A random element from the sequence.</returns>

        public static T RandomElementUsing<T>(this IEnumerable<T> sequence, Random rand) {
            int index = rand.Next(0, sequence.Count());
            return sequence.ElementAt(index);
        }

        /// <summary>
        /// Selects a specified number of random elements from the provided array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="sequence">The T[] array to select from.</param>
        /// <param name="number">The number of random elements to retrieve.</param>
        /// <returns>An IEnumerable collection containing the selected random elements.</returns>
        /// <remarks>
        /// This extension method provides a way to get multiple random elements from an array. It delegates the work to the `RandomElementsUsing` function, passing a newly created `Random` instance for internal use.
        /// </remarks>
        public static IEnumerable<T> RandomElements<T>(this T[] sequence, int number) => sequence.RandomElementsUsing<T>(number, new Random());

        /// <summary>
        /// Selects a specified number of random elements from the provided array using a specified Random instance.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="sequence">The T[] array to select from.</param>
        /// <param name="number">The number of random elements to retrieve.</param>
        /// <param name="rand">The Random instance to use for generating randomness (optional, new Random() used by default in RandomElements).</param>
        /// <returns>An IEnumerable collection containing the selected random elements.</returns>
        public static IEnumerable<T> RandomElementsUsing<T>(this T[] sequence, int number, Random rand) {
            number = Math.Min(number, sequence.Length);

            return Enumerable
                    .Range(0, number)
                    .Select(x => rand.Next(0, 1 + sequence.Length - number))
                    .OrderBy(x => x)
                    .Select((x, i) => sequence[x + i]);
        }

    }


}