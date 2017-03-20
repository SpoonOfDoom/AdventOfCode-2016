/*
 * Stolen from Samuel Jack: http://blog.functionalfun.net/2008/04/bouncing-on-your-tail.html
 */
using System;

namespace AdventOfCode2016.Tools
{
    public static class Trampoline
    {
        public static Func<T1, T2, TResult> MakeTrampoline<T1, T2, TResult>(this Func<T1, T2, Bounce<T1, T2, TResult>> function)
        {
            Func<T1, T2, TResult> trampolined = (T1 arg1, T2 arg2) =>
            {
                T1 currentArg1 = arg1;
                T2 currentArg2 = arg2;

                while (true)
                {
                    Bounce<T1, T2, TResult> result = function(currentArg1, currentArg2);

                    if (result.HasResult)
                    {
                        return result.Result;
                    }
                    else
                    {
                        currentArg1 = result.Param1;
                        currentArg2 = result.Param2;
                    }
                }
            };

            return trampolined;
        }

        public static Func<T1, TResult> MakeTrampoline<T1, TResult>(this Func<T1, Bounce<T1, TResult>> function)
        {
            Func<T1, TResult> trampolined = (T1 arg1) =>
            {
                T1 currentArg1 = arg1;

                while (true)
                {
                    Bounce<T1, TResult> result = function(currentArg1);

                    if (result.HasResult)
                    {
                        return result.Result;
                    }
                    else
                    {
                        currentArg1 = result.Param1;
                    }
                }
            };

            return trampolined;
        }

        public static Bounce<T1, T2, TResult> ReturnResult<T1, T2, TResult>(T1 placeHolder1, T2 placeHolder2, TResult result)
        {
            return new Bounce<T1, T2, TResult>(result);
        }

        public static Bounce<T1, TResult> ReturnResult<T1, T2, TResult>(T1 placeHolder, TResult result)
        {
            return new Bounce<T1, TResult>(result);
        }

        public static Bounce<T1, T2, TResult> Recurse<T1, T2, TResult>(T1 arg1, T2 arg2, TResult placeHolder)
        {
            return new Bounce<T1, T2, TResult>(arg1, arg2);
        }

        public static Bounce<T1, T2, TResult> Recurse<T1, T2, TResult>(T1 arg1, T2 arg2)
        {
            return new Bounce<T1, T2, TResult>(arg1, arg2);
        }

        public static Bounce<T1, T2, TResult> ReturnResult<T1, T2, TResult>(TResult result)
        {
            return new Bounce<T1, T2, TResult>(result);
        }

        public static Bounce<T1, TResult> Recurse<T1, TResult>(T1 arg1, TResult placeHolder)
        {
            return new Bounce<T1, TResult>(arg1);
        }

    }

    public struct Bounce<T1, TResult>
    {
        public T1 Param1 { get; private set; }
        public TResult Result { get; private set; }
        public bool HasResult { get; private set; }
        public bool Recurse { get; private set; }

        public Bounce(T1 param1) : this()
        {
            Param1 = param1;
            HasResult = false;
            Recurse = true;
        }

        public Bounce(TResult result) : this()
        {
            Result = result;
            Recurse = false;
        }
    }

    public struct Bounce<T1, T2, TResult>
    {
        public T1 Param1 { get; private set; }
        public T2 Param2 { get; private set; }
        public TResult Result { get; private set; }
        public bool HasResult { get; private set; }
        public bool Recurse { get; private set; }

        public Bounce(T1 param1, T2 param2) : this()
        {
            Param1 = param1;
            Param2 = param2;
            HasResult = false;
            Recurse = true;
        }

        public Bounce(TResult result) : this()
        {
            Result = result;
            HasResult = true;
            Recurse = false;
        }
    }


}
