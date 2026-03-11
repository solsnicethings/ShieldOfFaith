using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol.ShieldOfFaith
{
    public class OrderSensitiveElementComparer<T, E> : IEqualityComparer<T> where T : ICollection<E>
    {
        readonly IEqualityComparer<E> elementMatch;

        public OrderSensitiveElementComparer() => elementMatch = EqualityComparer<E>.Default;
        public OrderSensitiveElementComparer(IEqualityComparer<E> elementMatch) => this.elementMatch = elementMatch;

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
                return true;
            if (x == null || y == null)
                return false;
            if (x.Count != y.Count)
                return false;
            using (var a = x.GetEnumerator())
            using (var b = y.GetEnumerator())
                while (a.MoveNext() && b.MoveNext())
                    if (!elementMatch.Equals(a.Current, b.Current))
                        return false;
            return true;
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return obj == null ? 0 : obj.Count;
        }
    }
}
