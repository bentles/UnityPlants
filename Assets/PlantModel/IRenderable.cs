using System.Collections.Generic;
using UnityEngine;

internal interface IRenderable
{
    void Render(MeshData meshData, System.Random random, Vector3 translation, Quaternion rotation);
}