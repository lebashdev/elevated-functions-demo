using System;
using System.Diagnostics.CodeAnalysis;
using ElevatedFunctions.Core;

namespace ElevatedFunctions
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Program
    {
        static void Main()
        {
            // Say we have a "Toxic<T>" type, which is acts as a safe container
            // around a value of type T and isolated them from non-toxic values.
            // All operations against those "toxic" values should be performed within
            // the special container in order to not contaminate the rest of the logic.

            // We can trap values into the container with the following code:
            var a = Toxic.FromValue(2);
            var b = Toxic.FromValue(3);

            // Once a value has been put into a Toxic container, it should never leave it.

            // Given that information, let's think about how we can perform operation
            // against the trapped values.

            // Let's look at some functions:

            // Increment takes an integer, adds one to it, then returns the result as a normal integer.
            int Increment(int x) => x + 1;

            // ToxicIncrement also takes an integer, adds one to it, but returns a Toxic<int> rather than a normal integer.
            Toxic<int> ToxicIncrement(int x) => Toxic.FromValue(x + 1);

            // wrappedIncrement is the same function as Increment, but has itself been trapped in a Toxic container.
            var wrappedIncrement = Toxic.FromValue<Func<int, int>>(Increment);

            // Add is a normal function which takes two integers, add them together, then returns the result as a normal integer.
            int Add(int x, int y) => x + y;

            // Those functions are all variations of the same thing, with different signatures.
            // They all take at least one integer, apply a computation to it,
            // and return an integer of some form (either a normal value, or a trapped value).

            // To complete this exercise, figure out how to apply each of the four functions to the value of "a" (or both "a" and "b" in case of Add).
            // Because "a" and "b" are toxic values, you should not try to pull them outside of their containers.
            // You should apply the transformations inside the toxic contexts.
            // Conversely, the results of those computation are going to be inherently toxic
            // therefore they should be trapped in toxic containers themselves.
            // Note that it is OK to "transfer" a value from one toxic container to another, as long as the transfer occurs within a toxic context.
            // You may not change the way the four functions are implemented.
            // You may however write code around the functions, which will affect how the functions are invoked.
            // The resulting code should be type-safe (no reflection, etc.)

            // As an example, if you have a value Toxic(1), and you apply Increment to it, the result should be Toxic(2).
            // Because Increment only accepts a regular integer as its input, you need to figure out how to pass it a Toxic<int> instead.
            // And because Increment returns a regular integer, you also need to figure out how to get back a Toxic<int>.
            // That will involve writing one or more functions which will map from one invocation model to the other.

            // API:

            // The Toxic<T> type exposes a simple API:

            // - To create a toxic value, use Toxic.FromValue().

            // - To transform a value inside the toxic context, use Toxic.PassInto(x => ...), which takes a lambda parameter.
            //   The lambda will run inside the toxic context and receive the trapped value.
            //   The the result of the transformation will be converted to a Toxic<T>.

            // - If you end up with a nested value (e.g. Toxic<Toxic<T>>), you can flatten it be calling Unwrap().

            // - Calling ToString() on a toxic value will display the trapped value.  You may use it to show the output
            //   of the various function invocations.


            // Sample solutions:

            // map :: (a -> b) -> (Toxic a -> Toxic b)
            Func<Toxic<A>, Toxic<B>> Map<A, B>(Func<A, B> f)=> x => x.PassInto(f);

            // bind :: (a -> Toxic b) -> (Toxic a -> Toxic b)
            Func<Toxic<A>, Toxic<B>> Bind<A, B>(Func<A, Toxic<B>> f) => x => x.PassInto(f).Unwrap();

            // apply :: Toxic (a -> b) -> (Toxic a -> Toxic b)
            Func<Toxic<A>, Toxic<B>> Apply<A, B>(Toxic<Func<A, B>> f) => x => x.PassInto(xValue => f.PassInto(functionValue => functionValue(xValue))).Unwrap();

            // curry :: ((a, b) -> c) -> (a -> (b -> c))
            Func<A, Func<B, C>> Curry<A, B, C>(Func<A, B, C> f) => x => y => f(x, y);

            // Single parameter.
            var increment1 = Map<int, int>(Increment);
            var increment2 = Bind<int, int>(ToxicIncrement);
            var increment3 = Apply(wrappedIncrement);

            var u = increment1(a);
            var v = increment2(a);
            var w = increment3(a);

            Console.WriteLine(u);
            Console.WriteLine(v);
            Console.WriteLine(w);

            // Two parameters.
            var curried = Curry<int, int, int>(Add);
            var mapped = Map(curried);
            var partiallyApplied = mapped(a);
            var lifted = Apply(partiallyApplied);
            var z = lifted(b);

            Console.WriteLine(z);
        }
    }
}
