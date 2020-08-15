using System;

namespace ElevatedFunctions.Core
{
    public sealed class Toxic<T>
    {
        private readonly T _value;

        public Toxic(T value)
        {
            this._value = value;
        }

        public Toxic<TOut> RunInside<TOut>(Func<T, TOut> f) => new Toxic<TOut>(f(this._value));
        public override string ToString() => $"[Toxic {this._value}]";
    }

    public static class Toxic
    {
        public static Toxic<TValue> FromValue<TValue>(TValue value) => new Toxic<TValue>(value);
    }

    public static class ToxicExtensions
    {
        public static Toxic<T> Unwrap<T>(this Toxic<Toxic<T>> toxic)
        {
            Toxic<T> innerToxic = null;

            toxic.RunInside(innerValue =>
            {
                innerToxic = innerValue;
                return innerValue;
            });

            return innerToxic;
        }
    }
}
