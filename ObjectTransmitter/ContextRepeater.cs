using ObjectTransmitter.Collectors.Collections;
using ObjectTransmitter.Exceptions;
using ObjectTransmitter.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter
{
    public class ContextRepeater<T> where T : class
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

                // Checking type for dictionary.
                if (context is IRepeaterDictionaryApplier repeaterDictionary)
                {
                    PushDictionaryChanges(repeaterDictionary, changes, container);
                    return;
                }

                // Checking type registered as repeater in container.
                var contextType = context.GetType();
                if (!container.TryGetDescription(contextType, out var contextTypeDescription))
                    return;

                var propertyInfoByPropertyId = contextTypeDescription.Properties.ToDictionary(prop => prop.PropertyId, prop => prop.PropertyInfo);
                foreach (var change in changes)
                {
                    if (!propertyInfoByPropertyId.TryGetValue(change.PropertyId, out var propertyInfo))
                        throw new ObjectTransmitterException($"Property with id `{change.PropertyId}` not found for type `{contextType.FullName}`");

                    var isDictionary = propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IObservableDictionary<,>);

                    switch (change.ChangeType)
                    {
                        case ChangeType.ValueReset:
                            propertyInfo.SetValue(context, null);
                            break;

                        case ChangeType.ValueChanged:
                            if (isDictionary)
                            {
                                var dictType = typeof(RepeaterObservableDictionary<,>).MakeGenericType(propertyInfo.PropertyType.GetGenericArguments());
                                var dictionary = Activator.CreateInstance(dictType);
                                propertyInfo.SetValue(context, dictionary);
                                PushDictionaryChanges(dictionary as IRepeaterDictionaryApplier, change.ChildrenNodes, container);
                                break;
                            }

                            if (container.IsRepeaterRegistered(propertyInfo.PropertyType))
                            {
                                var newRepeater = container.CreateRepeaterInstance(propertyInfo.PropertyType);
                                propertyInfo.SetValue(context, newRepeater);
                                ApplyChanges(newRepeater, change.ChildrenNodes, container);
                                break;
                            }

                            var newValue = container.Deserialize(change.NewValue, change.PropertyId);
                            propertyInfo.SetValue(context, newValue);
                            break;

                        case ChangeType.ValueNotChanged:
                            if (isDictionary)
                            {
                                PushDictionaryChanges(propertyInfo.GetValue(context) as IRepeaterDictionaryApplier, change.ChildrenNodes, container);
                                break;
                            }

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

        private static void PushDictionaryChanges(
            IRepeaterDictionaryApplier repeaterDictionary, 
            IReadOnlyCollection<ContextChangedNode> changes, 
            ObjectTrasmitterContainer container)
        {
            if (repeaterDictionary == null)
                return;

            repeaterDictionary.GetTypes(out var keyType, out var valueType);
            var isValueRepeater = container.IsRepeaterRegistered(valueType);

            foreach (var change in changes)
            {
                var key = container.Deserialize(change.ItemKey, keyType);
                switch (change.ChangeType)
                {
                    case ChangeType.AddedOrUpdatedItem:
                        object value;
                        if (isValueRepeater)
                        {
                            value = container.CreateRepeaterInstance(valueType);
                            ApplyChanges(value, change.ChildrenNodes, container);
                        }
                        else
                        {
                            value = container.Deserialize(change.NewValue, valueType);
                        }

                        repeaterDictionary.AddOrUpdateItem(key, value);
                        break;
                    case ChangeType.RemovedItem:
                        repeaterDictionary.RemoveItem(key);
                        break;
                    case ChangeType.ValueNotChanged:
                        if (!isValueRepeater)
                            throw new ObjectTransmitterException($"Got {nameof(ChangeType)}.{nameof(ChangeType.ValueNotChanged)} with non-repeater value type");

                        var existsValue = repeaterDictionary.GetValue(key);
                        ApplyChanges(existsValue, change.ChildrenNodes, container);
                        break;
                    case ChangeType.ValueChanged:
                    default: throw new InvalidOperationException($"In dictionary got unexpected change type = {change.ChangeType}");
                }
            }
        }
    }
}
