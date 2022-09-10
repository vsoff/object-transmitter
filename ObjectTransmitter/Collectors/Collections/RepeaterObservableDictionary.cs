using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectTransmitter.Collectors.Collections
{
    internal interface IRepeaterDictionaryApplier
    {
        void GetTypes(out Type keyType, out Type valueType);
        void AddOrUpdateItem(object key, object value);
        void RemoveItem(object key);
        object GetValue(object key);
    }

    public class RepeaterObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>, IRepeaterDictionaryApplier
    {
        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private Action<CollectionChangeInfo<TKey, TValue>> _collectionChangedCallback;

        public int Count => _dictionary.Count;

        public RepeaterObservableDictionary()
        {
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public void AddOrUpdate(TKey key, TValue value)
        {
            _dictionary[key] = value;
            _collectionChangedCallback?.Invoke(new CollectionChangeInfo<TKey, TValue>(key, value, CollectionChangeType.AddOrUpdate));
        }

        public void Remove(TKey key)
        {
            _dictionary.Remove(key);
            _collectionChangedCallback?.Invoke(new CollectionChangeInfo<TKey, TValue>(key, default, CollectionChangeType.Remove));
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => AddOrUpdate(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void SetCallback(Action<CollectionChangeInfo<TKey, TValue>> collectionChangedCallback) => _collectionChangedCallback = collectionChangedCallback;

        public void AddOrUpdateItem(object key, object value) => AddOrUpdate((TKey)key, (TValue)value);

        public void RemoveItem(object key) => Remove((TKey)key);

        public object GetValue(object key) => this[(TKey)key];

        public void GetTypes(out Type keyType, out Type valueType)
        {
            keyType = typeof(TKey);
            valueType = typeof(TValue);
        }
    }

    public enum CollectionChangeType
    {
        AddOrUpdate,
        Remove,
    }

    public class CollectionChangeInfo<TKey, TValue>
    {
        public readonly TKey Key;
        public readonly TValue Value;
        public readonly CollectionChangeType ChangeType;

        public CollectionChangeInfo(TKey key, TValue value, CollectionChangeType changeType)
        {
            Key = key;
            Value = value;
            ChangeType = changeType;
        }
    }
}
