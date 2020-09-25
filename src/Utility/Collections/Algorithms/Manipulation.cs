using System;

using Utility.Collections.Interfaces;

namespace Utility.Collections.Algorithms
{
    public static class Manipulation
    {

        public static void FloodFill(IFillNode startNode, IComparable newValue)
        {
            IComparable oldValue = startNode.FillValue; //Save our old value.
            foreach (IFillNode ifnode in startNode.NodeConnectedNodes)
            {
                if (ifnode.FillValue.CompareTo(oldValue) == 0) //Connected node has the same old value
                {
                    FloodFill(ifnode, newValue); //Call the "child" node
                }
            }

            startNode.FillValue = newValue; //Finally change the value on our root(all the children are done)
        }

    }
}