using ObjectTransmitter.Reflection;
using System.Collections.Generic;

namespace ObjectTransmitter.Collectors
{
    public interface IRepeater
    {
        void ApplyChanges(IReadOnlyCollection<ContextChangedNode> changes, ObjectTrasmitterContainer container);
    }
}
