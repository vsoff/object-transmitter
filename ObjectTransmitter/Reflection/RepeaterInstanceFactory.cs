using ObjectTransmitter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter.Reflection
{
    internal class RepeaterInstanceFactory
    {
        private readonly IReadOnlyDictionary<Type, RepeaterFactory> _repeaterFactoriesByType;

        public RepeaterInstanceFactory(IEnumerable<RepeaterFactory> repeaterFactories)
        {
            if (repeaterFactories == null) throw new ArgumentNullException(nameof(repeaterFactories));

            _repeaterFactoriesByType = repeaterFactories.ToDictionary(x => x.RepeaterType, x => x);
        }

        public object CreateInstance(Type type)
        {
            if (!_repeaterFactoriesByType.TryGetValue(type, out var repeaterFactory))
                throw new ObjectTransmitterException($"Repeater factory for type `{type.FullName}` is not registered");

            return repeaterFactory.Create();
        }
    }
}
