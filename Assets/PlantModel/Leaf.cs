using System;
using System.Collections;
using System.Collections.Generic;
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

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        if (Age < 13)
        {
            var leafRandom = 0.8f + 0.2f * CachedRandomValue(0, random);
            RenderHelper.CreateLeaf(data, translation, rotation, (Age / 13f) * 0.5f * leafRandom);
        }
    }
}
