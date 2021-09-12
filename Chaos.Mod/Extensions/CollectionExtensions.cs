using System.Collections.Generic;

namespace Chaos.Mod.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddIfNotPresent<T>(this IList<T> array, T item)
        {
            if (!array.Contains(item)) array.Add(item);
        }
    }
}