namespace saper1.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<T> Where<T>(this T[,] array, Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(predicate);

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    T item = array[i, j];
                    if (predicate(item))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static T? FirstOrDefault<T>(this T[,] array, Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(predicate);

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    T item = array[i, j];
                    if (predicate(item))
                    {
                        return item;
                    }
                }
            }

            return default;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T[,] array)
        {
            ArgumentNullException.ThrowIfNull(array);

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    yield return array[i, j];
                }
            }
        }

        public static void ForEach<T>(this T[,] array, Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentNullException.ThrowIfNull(action);

            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    action(array[i, j]);
                }
            }
        }

        public static void Reset<T>(this T[,] array)
        {
            ArgumentNullException.ThrowIfNull(array);

            int rows = array.GetLength(0);

            int cols = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = default!;
                }
            }
        }
    }
}
