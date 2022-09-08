using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter.Collectors.Collections
{
    public class TransmitterObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>, ITransmitter
    {
        /// <remarks>
        /// Collection has empty property id, because it hasn't properties.
        /// </remarks>
        private const int EmptyPropertyId = -1;

        private readonly IDictionary<TKey, ContextChangedNode> _changedItemNodes = new Dictionary<TKey, ContextChangedNode>();
        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly ObjectTrasmitterContainer _container;

        private readonly bool _isValueTransmitter;

        public TransmitterObservableDictionary(ObjectTrasmitterContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _isValueTransmitter = container.TryGetTransmitterType<TValue>(out _);
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public void AddOrUpdate(TKey key, TValue value)
        {
            _dictionary[key] = value;

            byte[] keyData = _container.Serialize(key, typeof(TKey));
            if (_isValueTransmitter)
            {
                var innerChanges = (value as ITransmitter).CollectChanges(_container);
                _changedItemNodes[key] = new ContextChangedNode(EmptyPropertyId, null, keyData, ChangeType.AddedOrUpdatedItem, innerChanges);
            }
            else
            {
                var valueData = _container.Serialize(value, typeof(TValue));
                _changedItemNodes[key] = new ContextChangedNode(EmptyPropertyId, valueData, keyData, ChangeType.AddedOrUpdatedItem);
            }
        }

        public void Remove(TKey key)
        {
            _dictionary.Remove(key);
            var keyData = _container.Serialize(key, typeof(TKey));
            _changedItemNodes[key] = new ContextChangedNode(EmptyPropertyId, null, keyData, ChangeType.RemovedItem);
        }

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => AddOrUpdate(key, value);
        }

        public bool HasChanges(ObjectTrasmitterContainer container)
        {
            if (_changedItemNodes.Count != 0)
                return true;

            return _dictionary.Values.OfType<ITransmitter>().Any(transmitter => transmitter.HasChanges(container));
        }

        public void ClearChanges(ObjectTrasmitterContainer container)
        {
            _changedItemNodes.Clear();

            foreach (var item in _dictionary.Values.OfType<ITransmitter>())
            {
                item.ClearChanges(container);
            }
        }

        public IReadOnlyCollection<ContextChangedNode> CollectChanges(ObjectTrasmitterContainer container)
        {
            var result = new List<ContextChangedNode>();

            // Collect all removed items from dictionary.
            // SCENARIO: Item removed.
            result.AddRange(_changedItemNodes.Values.Where(x => x.ChangeType == ChangeType.RemovedItem));

            // Collect all updated items in dictionary.
            foreach (var keyWithValue in _dictionary)
            {
                IReadOnlyCollection<ContextChangedNode> changedSubItems = null;
                if (keyWithValue.Value is ITransmitter transmitterValue)
                    changedSubItems = transmitterValue.CollectChanges(container).ToArray();

                if (_changedItemNodes.TryGetValue(keyWithValue.Key, out var changedItem))
                {
                    // NOTE: Here can be only new added items, with/without changes.
                    if (changedItem.ChangeType != ChangeType.AddedOrUpdatedItem)
                        throw new ObjectTransmitterException($"Unexpected type of {nameof(ChangeType)}: {changedItem.ChangeType}");

                    if (changedSubItems == null || changedSubItems.Count == 0)
                    {
                        // SCENARIO: Item just added.
                        result.Add(changedItem);
                        continue;
                    }

                    // SCENARIO: Item added and updated.
                    result.Add(new ContextChangedNode(EmptyPropertyId, changedItem.NewValue, changedItem.ItemKey, ChangeType.AddedOrUpdatedItem, changedSubItems));
                    continue;
                }

                // SCENARIO: Item hasn't any changes.
                if (changedSubItems == null || changedSubItems.Count == 0)
                    continue;

                // SCENARIO: Item just updated.
                var keyData = container.Serialize(keyWithValue.Key, typeof(TKey));
                result.Add(new ContextChangedNode(EmptyPropertyId, null, keyData, ChangeType.ValueNotChanged, changedSubItems));
            }

            return result;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}
