using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Sol.ShieldOfFaith
{
    /// <summary>
    /// Concurrent dictionary where the keys are weakly referenced.
    /// When iterating the Keys or KeyValuePairs (not the underlying WeakReference.Keys/KeyValuePairs),
    /// invalid references are removed from the dictionary.
    /// 
    /// You can also call CleanCache() to achieve just the removal of no-longer-valid references.
    /// 
    /// If you try to get a value using this[K key] the dictionary will prefer returning null over throwing an exception when the value is not found.
    /// If the Value type cannot contain a null, then a KeyNotFoundException is thrown.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class ConcurrentWeakKeyDictionary<K, V> : ConcurrentDictionary<WeakReference<K>, V>, IDictionary<K, V>, IReadOnlyDictionary<K, V> where K : class
    {
        new public ICollection<K> Keys {
            get
            {
                var w = base.Keys;
                var r = new List<K>(w.Count);

                foreach (var k in w)
                    if (k.TryGetTarget(out var v))
                        r.Add(v);
                    else TryRemove(k, out var _x);

                return r;
            }
        }

        public bool IsReadOnly => false;
        public V this[K key]
        {
            get => TryGetValue(key, out var v) ? v : default(V) is object ? throw new KeyNotFoundException(key + " not found") : default(V);
            set => this[new WeakReference<K>(key)] = value;
        }

        public bool ContainsKey(K key) => ContainsKey(new WeakReference<K>(key));
        public void Add(K key, V value) { if (!TryAdd(new WeakReference<K>(key), value)) throw new ArgumentException(key + " already present"); }
        public void TryAdd(K key, V value) => TryAdd(new WeakReference<K>(key), value);

        public bool Remove(K key) => TryRemove(new WeakReference<K>(key), out var x_);
        public bool TryGetValue(K key, out V value) => TryGetValue(new WeakReference<K>(key), out value);
        public void Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);
        public bool Contains(KeyValuePair<K, V> item) => TryGetValue(item.Key, out var v) && EqualityComparer<V>.Default.Equals(v, item.Value);

        public void CleanCache()
        {
            foreach (var k in base.Keys)
                if (!k.TryGetTarget(out var k_))
                    TryRemove(k, out var x_);
        }
        public IEnumerable<KeyValuePair<K, V>> PresentItems
        {
            get
            {
                using (var e = base.GetEnumerator())
                    while (e.MoveNext())
                        if (e.Current.Key.TryGetTarget(out var k))
                            yield return new KeyValuePair<K, V>(k, e.Current.Value);
                        else TryRemove(e.Current.Key, out var x_);
            }
        }

        IEnumerable<K> IReadOnlyDictionary<K, V>.Keys => Keys;
        IEnumerable<V> IReadOnlyDictionary<K, V>.Values => Values;

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            foreach (var e in PresentItems)
                array[arrayIndex++] = e;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            var r = new WeakReference<K>(item.Key);
            if (TryGetValue(r, out var v) && EqualityComparer<V>.Default.Equals(v, item.Value))
                return TryRemove(r, out v);
            return false;
        }

        new public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => PresentItems.GetEnumerator();

        public bool TryRemove(K key, out V value) => TryRemove(new WeakReference<K>(key), out value);
        public bool TryUpdate(K key, V newValue, V comparisonValue) => TryUpdate(new WeakReference<K>(key), newValue, comparisonValue);

        public V AddOrUpdate<TArg>(K key, Func<K, TArg, V> addValueFactory, Func<K, V, TArg, V> updateValueFactory, TArg factoryArgument)
        {
            var addValueFactory_ = (Func<WeakReference<K>, TArg, V>)((k, v) =>
            {
                //if (!k.TryGetTarget(out var t)) t = null;
                //return addValueFactory(t, v);
                return addValueFactory(key, v);
            });
            var updateValueFactory_ = (Func<WeakReference<K>, V, TArg, V>)((k, v, a) =>
            {
                //if (!k.TryGetTarget(out var t)) t = null;
                //return updateValueFactory(t, v, a);
                return updateValueFactory(key, v, a);
            });
            return AddOrUpdate(new WeakReference<K>(key), addValueFactory_, updateValueFactory_, factoryArgument);
        }
        public V AddOrUpdate(K key, Func<K, V> addValueFactory, Func<K, V, V> updateValueFactory)
        {
            var addValueFactory_ = (Func<WeakReference<K>, V>)(k =>
            {
                //if (!k.TryGetTarget(out var t)) t = null;
                //return addValueFactory(t);
                return addValueFactory(key);
            });
            var updateValueFactory_ = (Func<WeakReference<K>, V, V>)((k, a) =>
            {
                //if (!k.TryGetTarget(out var t)) t = null;
                //return updateValueFactory(t, a);
                return updateValueFactory(key, a);
            });
            return AddOrUpdate(new WeakReference<K>(key), addValueFactory_, updateValueFactory_);
        }
        public V AddOrUpdate(K key, V addValue, Func<K, V, V> updateValueFactory)
        {
            var updateValueFactory_ = (Func<WeakReference<K>, V, V>)((k, a) =>
            {
                //if (!k.TryGetTarget(out var t)) t = null;
                //return updateValueFactory(t, a);
                return updateValueFactory(key, a);
            });
            return AddOrUpdate(new WeakReference<K>(key), addValue, updateValueFactory_);
        }

        public V GetOrAdd(K key, Func<K, V> valueFactory)
        {
            var valueFactory_ = (Func<WeakReference<K>, V>)((k) => {
                return valueFactory(key);
            });
            return GetOrAdd(new WeakReference<K>(key), valueFactory_);
        }
        public V GetOrAdd(K key, V value) => GetOrAdd(new WeakReference<K>(key), value);
        public V GetOrAdd<TArg>(K key, Func<K, TArg, V> valueFactory, TArg factoryArgument)
        {
            var valueFactory_ = (Func<WeakReference<K>, TArg, V>)((k, v) => {
                return valueFactory(key, v);
            });
            return GetOrAdd(new WeakReference<K>(key), valueFactory_, factoryArgument);
        }

        public ConcurrentWeakKeyDictionary() : base(new RetainedWeakReferenceEqualityComparer<K>())
        {            
        }
        public ConcurrentWeakKeyDictionary(IEqualityComparer<K> keyComparer) : base(new RetainedWeakReferenceEqualityComparer<K>(keyComparer))
        {
        }
        public ConcurrentWeakKeyDictionary(int concurrencyLevel, int capacity, IEqualityComparer<K> keyComparer) : base(concurrencyLevel, capacity, new RetainedWeakReferenceEqualityComparer<K>(keyComparer))
        {
        }
        public ConcurrentWeakKeyDictionary(int concurrencyLevel, int capacity) : base(concurrencyLevel, capacity, new RetainedWeakReferenceEqualityComparer<K>())
        {
        }
    }
}
