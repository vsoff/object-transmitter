using ObjectTransmitter.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter.Collectors
{
    public class Transmitter : ITransmitter
    {
        internal const string SaveChangeMethodName = nameof(SaveChange);
        private readonly IDictionary<int, object> _changes = new Dictionary<int, object>();

        public IReadOnlyCollection<ContextChangedNode> CollectChanges(ObjectTrasmitterContainer container)
        {
            var changes = new List<ContextChangedNode>();
            var processedPropertyIds = new HashSet<int>();

            foreach (var change in _changes)
            {
                processedPropertyIds.Add(change.Key);
                if (change.Value == null)
                {
                    changes.Add(new ContextChangedNode(change.Key, null, null, ChangeType.ValueReset));
                }
                else if (change.Value is ITransmitter transmitter)
                {
                    var innerChanges = transmitter.CollectChanges(container);
                    changes.Add(new ContextChangedNode(change.Key, null, null, ChangeType.ValueChanged, innerChanges));
                }
                else
                {
                    var value = container.Serialize(change.Value, change.Key);
                    changes.Add(new ContextChangedNode(change.Key, value, null, ChangeType.ValueChanged));
                }
            }

            foreach (var transmitter in GetPropertiesTransmitters(container))
            {
                // Skip already processed properties.
                if (processedPropertyIds.Contains(transmitter.Key))
                    continue;

                var innerTransmitterChanges = transmitter.Value.CollectChanges(container);
                if (innerTransmitterChanges.Count == 0)
                    continue;

                changes.Add(new ContextChangedNode(transmitter.Key, null, null, ChangeType.ValueNotChanged, innerTransmitterChanges));
            }

            return changes;
        }

        public void ClearChanges(ObjectTrasmitterContainer container)
        {
            _changes.Clear();

            foreach (var transmitter in GetPropertiesTransmitters(container))
                transmitter.Value.ClearChanges(container);
        }

        public bool HasChanges(ObjectTrasmitterContainer container)
            => _changes.Count > 0 || GetPropertiesTransmitters(container).Any(transmitter => transmitter.Value.HasChanges(container));

        protected void SaveChange<T>(int propertyId, T newValue)
        {
            _changes[propertyId] = newValue;
        }

        // TODO: Add cache.
        // TODO: Check possibility for inject container in constructor.
        private IEnumerable<KeyValuePair<int, ITransmitter>> GetPropertiesTransmitters(ObjectTrasmitterContainer container)
        {
            // This is not optimal way, but I'll fix it in future.
            var typeDescription = container.GetDescription(GetType());
            foreach (var property in typeDescription.Properties)
            {
                var transmitter = property.PropertyInfo.GetValue(this) as ITransmitter;
                if (transmitter != null)
                    yield return new KeyValuePair<int, ITransmitter>(property.PropertyId, transmitter);
            }
        }
    }
}
