using System;
using System.Threading.Tasks;

namespace ElevatedFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Task.FromResult(2);
            var b = Task.FromResult(3);

            // Use these to increment "a".
            int Increment(int x) => x + 1;
            Task<int> IncrementAsync(int x) => Task.FromResult(x + 1);
            var wrappedIncrement = Task.FromResult<Func<int, int>>(Increment);

            // Use the function below to add "a" to "b".
            int Add(int x, int y) => x + y;

            Func<Task<A>, Task<B>> Map<A, B>(Func<A, B> f)=> x => x.ContinueWith(t => f(t.Result));
            Func<Task<A>, Task<B>> Bind<A, B>(Func<A, Task<B>> f) => x => x.ContinueWith(t => f(t.Result)).Unwrap();
            Func<Task<A>, Task<B>> Apply<A, B>(Task<Func<A, B>> f) => x => x.ContinueWith(tX => f.ContinueWith(tF => tF.Result(tX.Result))).Unwrap();
            Func<A, Func<B, C>> Curry<A, B, C>(Func<A, B, C> f) => x => y => f(x, y);

            void Print<A>(Task<A> x) => Console.WriteLine(x.Result);

            // Single parameter.
            var increment1 = Map<int, int>(Increment);
            var increment2 = Bind<int, int>(IncrementAsync);
            var increment3 = Apply(wrappedIncrement);

            var u = increment1(a);
            var v = increment2(a);
            var w = increment3(a);

            Print(u);
            Print(v);
            Print(w);

            // Two parameters.
            var curried = Curry<int, int, int>(Add);
            var mapped = Map(curried);
            var partiallyApplied = mapped(a);
            var lifted = Apply(partiallyApplied);
            var z = lifted(b);

            Print(z);
        }
    }
}
