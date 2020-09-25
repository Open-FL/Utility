using System;

using Utility.Collections.Enums;

namespace Utility.Collections.Interfaces
{
    public interface INode : IComparable<INode>
    {

        NodeState NodeState { get; set; }

        float NodeCost { get; }

        INode NodeParentNode { get; set; }

        float NodeCurrentCost { get; set; }

        float NodeEstimatedCost { get; set; }

        bool NodeIsActive { get; }

        INode[] ConnectedNodes { get; }

        IVec3 NodePosition { get; }

    }
}