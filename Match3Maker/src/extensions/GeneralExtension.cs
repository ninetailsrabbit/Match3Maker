
namespace SystemExtensions {

    public static class GeneralExtension {

        public static bool In<T>(this T val, params T[] values) where T : struct {
            return values.Contains(val);
        }

        public static bool In<T>(this T val, IEnumerable<T> values) where T : struct {
            return values.Contains(val);
        }

    }
}
