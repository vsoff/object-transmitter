using ObjectTransmitter.Reflection;
using System.Collections.Generic;

namespace ObjectTransmitter.Collectors
{
    public interface ITransmitter
    {
        bool HasChanges(ObjectTrasmitterContainer container);
        void ClearChanges(ObjectTrasmitterContainer container);
        IReadOnlyCollection<ContextChangedNode> CollectChanges(ObjectTrasmitterContainer container);
    }
}
