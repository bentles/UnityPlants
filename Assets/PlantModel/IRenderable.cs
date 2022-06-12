using Assets.PlantModel;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

internal interface IRenderable
{
    void Render(MeshData meshData, RenderContext renderContext, CancellationToken ct);
}