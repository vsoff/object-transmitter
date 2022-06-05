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

        internal int GetChangedNodesCount() => ChangedNodes.Sum(GetChangedNodesCount);

        private int GetChangedNodesCount(ContextChangedNode node)
        {
            var count = 0;

            if (node.ChangeType != ChangeType.ValueNotChanged)
            {
                count++;
            }

            foreach (var childNode in node.ChildrenNodes)
            {
                count += GetChangedNodesCount(node);
            }

            return count;
        }
    }
}
