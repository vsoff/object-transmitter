using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectTransmitter.Collectors
{
    public class Repeater : IRepeater
    {
        internal const string PropertyChangedMethodName = nameof(PropertyChanged);

        public void ApplyChanges(IReadOnlyCollection<ContextChangedNode> changes, ObjectTrasmitterContainer container)
        {
            if (changes.Count == 0)
                return;

            var propertyInfoByPropertyId = GetPropertyInfoByIdMap(container);
            foreach (var change in changes)
            {
                if (!propertyInfoByPropertyId.TryGetValue(change.PropertyId, out var propertyInfo))
                    throw new ObjectTransmitterException($"Property with id `{change.PropertyId}` not found for type `{GetType().FullName}`");

                switch (change.ChangeType)
                {
                    case ChangeType.ValueChanged:
                        var newValue = container.Deserialize(change.NewValue, change.PropertyId);
                        propertyInfo.SetValue(this, newValue);
                        break;
                    case ChangeType.ValueNotChanged:
                        (propertyInfo.GetValue(this) as IRepeater)?.ApplyChanges(change.ChildrenNodes, container);
                        break;
                    default: throw new ObjectTransmitterException($"Got unexpected {nameof(ChangeType)}: {change.ChangeType}");
                }
            }
        }

        protected void PropertyChanged<T>(int propertyId, T newValue)
        {
            
        }

        // TODO: Add cache.
        private IReadOnlyDictionary<int, PropertyInfo> GetPropertyInfoByIdMap(ObjectTrasmitterContainer container)
        {
            // This is not optimal way, but I'll fix it in future.
            var typeDescription = container.GetDescription(GetType());
            return typeDescription.Properties.ToDictionary(prop => prop.PropertyId, prop => prop.PropertyInfo);
        }
    }
}
