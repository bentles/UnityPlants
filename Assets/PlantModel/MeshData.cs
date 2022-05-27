using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> Vertices { get; set; }
    public List<int> Triangles { get; set; }
    public List<int> LeafTriangles { get; set; }

    public void Clear()
    {
        Vertices.Clear();
        Triangles.Clear();
        LeafTriangles.Clear();
    }
}
