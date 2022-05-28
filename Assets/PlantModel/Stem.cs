using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stem : Growable
{
    public Stem(Growable child, Plant plant): base(plant)
    {
        Child = child;
        child.Reset();
    }

    /// <summary>
    /// Create a stem with a tip
    /// </summary>
    public Stem(Plant plant) : base(plant)
    {
        Child = new Tip(this, Plant);
    }

    public override Growable Child { get; set; }


    public override bool ChildGrowth(float time)
    {
        return Child.Grow(time);
    }

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        var radial = Quaternion.AngleAxis(CachedRandomValue(0, random) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(CachedRandomValue(1, random) * 25f, rotation * radial * Vector3.forward);

        var offset = Height;
        RenderHelper.CreateBranchSegment(data, translation, rotation, this, height: offset);

        Child.Render(data, random, translation + rotation * new Vector3(0f, offset, 0f), ang * rotation);
    }
}
