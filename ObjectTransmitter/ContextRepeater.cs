using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter
{
    public class ContextRepeater<T>
        where T : class
    {
        private readonly ObjectTrasmitterContainer _container;

        internal ContextRepeater(T context, ObjectTrasmitterContainer container)
        {
            Context = context;
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public T Context { get; }

        public void ApplyChanges(ContextChangesRoot changes)
        {
            if (changes == null) throw new ArgumentNullException(nameof(changes));

            ApplyChanges(Context, changes.ChangedNodes, _container);
        }

        private static void ApplyChanges(object context, IReadOnlyCollection<ContextChangedNode> changes, ObjectTrasmitterContainer container)
        {
            try
            {
                if (context == null || changes.Count == 0)
                    return;

                // Checking type registered as repeater in container.
                var contextType = context.GetType();
                if (!container.TryGetDescription(contextType, out var contextTypeDescription))
                    return;

                var propertyInfoByPropertyId = contextTypeDescription.Properties.ToDictionary(prop => prop.PropertyId, prop => prop.PropertyInfo);
                foreach (var change in changes)
                {
                    if (!propertyInfoByPropertyId.TryGetValue(change.PropertyId, out var propertyInfo))
                        throw new ObjectTransmitterException($"Property with id `{change.PropertyId}` not found for type `{contextType.FullName}`");

                    switch (change.ChangeType)
                    {
                        case ChangeType.ValueChanged:
                            var newValue = container.Deserialize(change.NewValue, change.PropertyId);
                            propertyInfo.SetValue(context, newValue);
                            break;

                        case ChangeType.ValueNotChanged:
                            var value = propertyInfo.GetValue(context);
                            ApplyChanges(value, change.ChildrenNodes, container);
                            break;

                        default: throw new ObjectTransmitterException($"Got unexpected {nameof(ChangeType)}: {change.ChangeType}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ObjectTransmitterException("Occured exception while applying changes", ex);
            }
        }
    }
}
