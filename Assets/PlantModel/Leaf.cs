using Assets.PlantModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Leaf : Growable
{
    public Leaf(Plant plant) : base(plant)
    {
    }

    public override Growable Child { get; set; } = null;

    public override bool ChildGrowth(float time)
    {
        return false;
    }

    public override void Render(MeshData data, System.Random random, RenderContext renderContext, CancellationToken ct)
    {
        if (Age < 13)
        {
            var leafRandom = 0.8f + 0.2f * CachedRandomValue(0, random);
            RenderHelper.CreateLeaf(data, renderContext.Translation, renderContext.Rotation, (Age / 13f) * 0.5f * leafRandom);
        }
    }
}
