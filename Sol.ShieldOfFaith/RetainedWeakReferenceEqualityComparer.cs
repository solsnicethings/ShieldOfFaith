using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol.ShieldOfFaith
{
    public class RetainedWeakReferenceEqualityComparer<T> : IEqualityComparer<WeakReference<T>> where T : class
    {
        public readonly IEqualityComparer<T> ReferenceEquality;
        
        public RetainedWeakReferenceEqualityComparer(IEqualityComparer<T> referenceEquality)
        {
            ReferenceEquality = referenceEquality;
        }
        public RetainedWeakReferenceEqualityComparer() : this(EqualityComparer<T>.Default) { }

        bool IEqualityComparer<WeakReference<T>>.Equals(WeakReference<T> x, WeakReference<T> y)
        {
            return ReferenceEquals(x, y) ||
                (x != null && y != null &&
                x.TryGetTarget(out var a) && y.TryGetTarget(out var b) && ReferenceEquality.Equals(a, b));
        }

        int IEqualityComparer<WeakReference<T>>.GetHashCode(WeakReference<T> obj)
        {
            return obj != null && obj.TryGetTarget(out var v) && v != null ? v.GetHashCode() : 0;
        }
    }
}
