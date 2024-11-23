using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestInheritanceGenerator
{
    internal class ValueSemanticsList<T> : IEnumerable<T>
    {
        private readonly List<T> actual = new();

        public int Count => actual.Count;

        public void Add(T item)
        {
            actual.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)actual).GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)actual).GetEnumerator();
        }

        public override bool Equals(object? other)
        {
            return other is ValueSemanticsList<T> list &&
                actual.SequenceEqual(list.actual);
        }

        public override int GetHashCode()
        {
            int hashCode = 1502939027;
            foreach (var item in actual)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(item);
            }
            return hashCode;
        }
    }
}
