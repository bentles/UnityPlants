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

    public override bool ChildGrowth()
    {
        return false;
    }

    public override void Render(MeshData data, System.Random random, RenderContext renderContext, CancellationToken ct)
    {
        if (Age < 13)
        {
            var leafRandom = 0.9f + 0.1f * CachedRandomValue(0, random);
            RenderHelper.CreateLeaf(data, renderContext.Translation, renderContext.Rotation, (Age / 13f) * 0.4f * leafRandom);
        }
    }
}
