using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// T -> ST | NST

public class Tip : Growable
{
    public Tip(Stem parent, Plant plant) : base(plant)
    {
        Parent = parent;
    }

    public Stem Parent { get; private set; }
    public override Growable Child { get; set; } = null;

    public override bool Grow(float time)
    {
        base.Grow(time);

        //decide when to make chidrens
        NaiveGrowthLogic();

        return JustSprouted;
    }

    private void NaiveGrowthLogic()
    {
        if (Age >= 2 && !Sprouted)
        {
            Sprouted = true;
            var rand = UnityEngine.Random.Range(0, 1f);
            if (rand < 0.1f)
            {
                StemTip();
            }
            else
            {
                NodeStemTip();
            }
        }
    }

    private void StemTip()
    {
        var stem = new Stem(this, Plant);
        Parent.Child = stem;
        Parent = stem;
    }

    private void NodeStemTip()
    {
        var stem = new Stem(this, Plant);
        Parent.Child = new Node(stem, Plant);
        Parent = stem;
    }

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        RenderHelper.CreateCylinder(data, translation, rotation, this, height: 0.2f * Height);
    }
}
