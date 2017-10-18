using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2016.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> DifferentCombinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).DifferentCombinations(k - 1).Select(c =>
                    (new[] { e }).Concat(c)));
        }

        public static IEnumerable<IEnumerable<T>> AllCombinations<T>(this IEnumerable<T> elements, int k, bool orderMatters)
        {
            return k == 0 ? new[] { new T[0] } :
                elements.SelectMany((e, i) =>
                    elements.Take(i).Concat(elements.Skip(i+1)).DifferentCombinations(k - 1).Select(c =>
                        (new[] { e }).Concat(c)));
        }
    }
}
