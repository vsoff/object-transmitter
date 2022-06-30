using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectTransmitter
{
    public class ContextChangesRoot
    {
        internal readonly IReadOnlyCollection<ContextChangedNode> ChangedNodes;

        internal ContextChangesRoot(IReadOnlyCollection<ContextChangedNode> changedNodes)
        {
            ChangedNodes = changedNodes ?? Array.Empty<ContextChangedNode>();
        }
    }
}
