using System.Collections.Generic;

using Utility.Collections.Interfaces;

namespace Utility.Collections
{
    public static class PathFinding
    {

        #region Pathfinding

        public static List<INode> FindPath(INode from, INode to)
        {
            INode current;
            PriorityQueue<INode> connectedNodes = new PriorityQueue<INode>();
            foreach (INode inode in from.ConnectedNodes)
            {
                connectedNodes.Enqueue(inode);
            }

            List<INode> doneNodes = new List<INode>();
            while (true)
            {
                if (connectedNodes.Count == 0)
                {
                    ResetNodes(doneNodes);
                    ResetNodes(connectedNodes.GetData());
                    return new List<INode>();
                }

                current = connectedNodes.Dequeue();
                doneNodes.Add(current);
                current.NodeState = Enums.NodeState.Closed;

                if (current == to)
                {
                    //Generate Path
                    List<INode> ret = GenerateNodePath(to);

                    //Reset Node Graph
                    ResetNodes(doneNodes);
                    ResetNodes(connectedNodes.GetData());
                    return ret; //Generated Path
                }

                for (int i = 0; i < current.ConnectedNodes.Length; i++)
                {
                    INode connected = current.ConnectedNodes[i];
                    if (!connected.NodeIsActive || connected.NodeState == Enums.NodeState.Closed)
                    {
                        continue;
                    }

                    if (connected.NodeState == Enums.NodeState.Untested)
                    {
                        connected.NodeParentNode = current;
                        connected.NodeCurrentCost =
                            current.NodeCurrentCost +
                            VectorMath.GetDistance(current.NodePosition, connected.NodePosition) *
                            connected.NodeCost;
                        connected.NodeEstimatedCost =
                            connected.NodeCurrentCost +
                            VectorMath.GetDistance(connected.NodePosition, to.NodePosition);
                        connected.NodeState = Enums.NodeState.Open;
                        connectedNodes.Enqueue(connected);
                    }

                    if (current != connected)
                    {
                        float newCostCurrent = current.NodeCurrentCost +
                                               VectorMath.GetDistance(current.NodePosition, connected.NodePosition);
                        if (newCostCurrent < connected.NodeCurrentCost)
                        {
                            connected.NodeParentNode = current;
                            connected.NodeCurrentCost = newCostCurrent;
                        }
                    }
                }
            }
        }

        #endregion

        public static void ResetNodes(List<INode> nodes)
        {
            foreach (INode node in nodes)
            {
                node.NodeCurrentCost = 0;
                node.NodeEstimatedCost = 0;
                node.NodeParentNode = null;
                node.NodeState = Enums.NodeState.Untested;
            }
        }

        public static List<INode> GenerateNodePath(INode targetNode)
        {
            List<INode> ret = new List<INode>();
            INode current = targetNode;
            while (current != null)
            {
                ret.Add(current);
                current = current.NodeParentNode;
            }

            ret.Reverse();
            return ret;
        }

    }
}