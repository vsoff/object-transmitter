using System;
using System.Collections.Generic;

namespace ObjectTransmitter
{
    public class ContextChangedNode
    {
        public readonly int PropertyId;
        public readonly byte[] NewValue;
        public readonly byte[] ItemKey;
        public readonly ChangeType ChangeType;
        public readonly IReadOnlyCollection<ContextChangedNode> ChildrenNodes;

        public ContextChangedNode(
            int propertyId,
            byte[] newValue,
            byte[] itemKey,
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
