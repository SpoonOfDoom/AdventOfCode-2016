using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Extensions
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

        public static IEnumerable<IEnumerable<T>> AllCombinations<T>(this IEnumerable<T> elements, bool orderMatters)
        {
            List<IEnumerable<T>> ret = new List<IEnumerable<T>>();

            foreach (var e in elements)
            {
                List<T> tList = new List<T>() {e};
                
            }

            return ret;
        }
    }
}
