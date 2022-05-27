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


    public override bool Grow(float time)
    {
        if (Age >= Plant.MaxAge)
        {
            return false;
        }

        base.Grow(time);
        return Child.Grow(time);
    }

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        // the fact that these can rotate 360 deg looks really bad now
        var radial = Quaternion.AngleAxis(CachedRandomValue(0, random) * 360f, rotation * Vector3.up);
        var ang = Quaternion.AngleAxis(CachedRandomValue(1, random) * 25f, rotation * Vector3.forward);


        var offset = Height;
        RenderHelper.CreateCylinder(data, translation, rotation, this, height: offset);

        Child.Render(data, random, translation + rotation * new Vector3(0f, offset, 0f), radial * ang * rotation);
    }
}
