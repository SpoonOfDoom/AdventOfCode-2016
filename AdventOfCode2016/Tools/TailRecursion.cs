/*
 * Stolen from Thomas Levesque https://www.thomaslevesque.com/2011/09/02/tail-recursion-in-c/
 */

using System;

namespace AdventOfCode2016.Tools
{
    public static class TailRecursion
    {
        public static T Execute<T>(Func<RecursionResult<T>> func)
        {
            do
            {
                RecursionResult<T> recursionResult = func();
                if (recursionResult.IsFinalResult)
                    return recursionResult.Result;
                func = recursionResult.NextStep;
            } while (true);
        }

        public static RecursionResult<T> Return<T>(T result)
        {
            return new RecursionResult<T>(true, result, null);
        }

        public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> nextStep)
        {
            return new RecursionResult<T>(false, default(T), nextStep);
        }

    }

    public class RecursionResult<T>
    {
        internal RecursionResult(bool isFinalResult, T result, Func<RecursionResult<T>> nextStep)
        {
            IsFinalResult = isFinalResult;
            Result = result;
            NextStep = nextStep;
        }

        public bool IsFinalResult { get; }
        public T Result { get; }
        public Func<RecursionResult<T>> NextStep { get; }
    }
}