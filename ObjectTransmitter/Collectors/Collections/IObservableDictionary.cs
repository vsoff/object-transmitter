using System.Collections.Generic;

namespace ObjectTransmitter.Collectors.Collections
{
    // TODO: Add restrictions for key values, it must be simple like int/long/guid.
    public interface IObservableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        bool TryGetValue(TKey key, out TValue value);
        void AddOrUpdate(TKey key, TValue value);
        void Remove(TKey key);
        bool ContainsKey(TKey key);
        TValue this[TKey key] { get; set; }
        int Count { get; }
    }
}
