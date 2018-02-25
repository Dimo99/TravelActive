using System.Collections.Generic;

namespace TravelActive.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddMany<T>(this List<T> list,params T[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                list.Add(items[i]);
            }
        }
    }
}