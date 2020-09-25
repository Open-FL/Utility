using System;
using System.Collections.Generic;

namespace Utility.Collections.Interfaces
{
    public interface IMesh
    {

        List<IVec3> Vertices { get; set; }

        List<IVec3> Normals { get; set; }

        List<IVec2> UVs { get; set; }

        List<Tuple<int, int, int>> Indices { get; set; }

        IMesh GetEmptyMesh();

    }
}