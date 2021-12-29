using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.Extensions
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            _ = list.Select((elem, index) =>
            {
                action(elem, index);
                return 0;
            }).ToList();
        }
        
        public static IList<T> ChooseRandomItems<T>(this IList<T> source, int count, Random? random = null)
        {
            if (count >= source.Count)
            {
                return source;
            }
            random ??= new Random();
            var idx = 0;
            var result = new HashSet<T>();
            while (result.Count != count)
            {
                if (random.Next(0, 10) < 5)
                {
                    idx++;
                    continue;
                }
                result.Add(source[idx % source.Count]);
            }
            return result.ToList();
        }
    }
}