using System;

namespace Utility.Collections.Interfaces
{
    public interface IFillNode
    {

        IFillNode[] NodeConnectedNodes { get; }

        IComparable FillValue { get; set; }

    }
}