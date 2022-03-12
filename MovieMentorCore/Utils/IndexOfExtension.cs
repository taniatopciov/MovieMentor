namespace System.Collections.Generic;

public static class IndexOfExtension
{
    public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
    {
        for (var index = 0; index < list.Count; index++)
        {
            if (predicate.Invoke(list[index]))
            {
                return index;
            }
        }

        return -1;
    }
}
