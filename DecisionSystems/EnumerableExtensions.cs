using System;
using System.Collections.Generic;
using System.Linq;

namespace DecisionSystems
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TOut> Pairwise<TIn, TOut>(this IEnumerable<TIn> items, Func<TIn, TIn, TOut> combine)
        {
            return items.Zip(items.Skip(1), combine);
        }

        public static T[] Shuffle<T>(this IEnumerable<T> items)
        {
            var array = items.ToArray();
            var random = new Random();
            for(var i = 0; i < array.Length - 1; i++)
            {
                var j = random.Next(i, array.Length);
                var tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
            return array;
        }

        public static T Best<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> selector, Func<TCompare, TCompare, bool> isFirstBetter) where TCompare : IComparable<TCompare>
        {
            var best = default(T);
            var hasBest = false;
            foreach (var item in items)
            {
                if (!hasBest || isFirstBetter(selector(item), selector(best)))
                {
                    best = item;
                    hasBest = true;
                }
            }
            if (!hasBest)
            {
                throw new ArgumentException("Collection is empty");
            }
            return best;
        }

        public static T MinBy<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> selector) where TCompare : IComparable<TCompare>
        {
            return items.Best(selector, (a, b) => a.CompareTo(b) < 0);
        }

        public static T MaxBy<T, TCompare>(this IEnumerable<T> items, Func<T, TCompare> selector) where TCompare : IComparable<TCompare>
        {
            return items.Best(selector, (a, b) => a.CompareTo(b) > 0);
        }
    }
}