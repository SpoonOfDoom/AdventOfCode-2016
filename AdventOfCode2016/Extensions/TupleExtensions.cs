using System;

namespace AdventOfCode2016.Extensions
{
    public static class TupleExtensions
    {
        public static Tuple<T, T> Flip<T>(this Tuple<T, T> tuple)
        {
            Tuple<T, T> newTuple = Tuple.Create(tuple.Item2, tuple.Item1);
            return newTuple;
        }
    }
}
