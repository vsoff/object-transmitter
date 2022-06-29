using System;
using System.Collections.Generic;

namespace ObjectTransmitter
{
    public class ContextChangedNode
    {
        public readonly int PropertyId;
        public readonly object NewValue;
        public readonly object ItemKey;
        public readonly ChangeType ChangeType;
        public readonly IReadOnlyCollection<ContextChangedNode> ChildrenNodes;

        public ContextChangedNode(ContextChangedNode node, object newValue, IReadOnlyCollection<ContextChangedNode> changedChildren) : this(
            node.PropertyId,
            newValue,
            node.ItemKey,
            node.ChangeType,
            changedChildren)
        {
        }

        public ContextChangedNode(
            int propertyId,
            object newValue,
            object itemKey,
            ChangeType changeType,
            IReadOnlyCollection<ContextChangedNode> changedChildren = null)
        {
            PropertyId = propertyId;
            NewValue = newValue;
            ItemKey = itemKey;
            ChangeType = changeType;
            ChildrenNodes = changedChildren ?? Array.Empty<ContextChangedNode>();
        }
    }
}
