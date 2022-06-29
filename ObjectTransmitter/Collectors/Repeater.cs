using ObjectTransmitter.Reflection;
using System.Collections.Generic;

namespace ObjectTransmitter.Collectors
{
    public class Repeater : IRepeater
    {
        internal const string PropertyChangedMethodName = nameof(PropertyChanged);

        public void ApplyChanges(IReadOnlyCollection<ContextChangedNode> changes, ObjectTrasmitterContainer container)
        {
            throw new System.NotImplementedException();
        }

        protected void PropertyChanged<T>(int propertyId, T newValue)
        {
            
        }
    }
}
