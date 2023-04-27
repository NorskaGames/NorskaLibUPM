using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TO DO:
// Is this really usable?
namespace NorskaLib.Extensions
{
    public struct ValuePair<T> : IEnumerable<T> where T : IEquatable<T>, IComparable<T>
    {
        private readonly T a;
        private readonly T b;

        public ValuePair(T a, T b)
        {
            this.a = a;
            this.b = b;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return a;
            yield return b;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return a;
            yield return b;
        }

        public T Other(T value)
        {
            if (value.Equals(a))
                return b;

            if (value.Equals(b))
                return a;

            throw new System.ArgumentException("Argument doesn't match any of the ValuePair values.");
        }
    }
}
