using System.Collections.Generic;

namespace DecisionSystems
{
    public static class ArrayExtensions
    {
        public static void Swap<T>(this IList<T> list, int idx1, int idx2)
        {
            (list[idx1], list[idx2]) = (list[idx2], list[idx1]);
        }
    }
}