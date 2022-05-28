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


    public override bool ChildGrowth(float time)
    {
        NaiveGrowthLogic(time);

        return JustSprouted; //hmm
    }

    private void NaiveGrowthLogic(float time)
    {
        if (Age >= 2 && !Sprouted)
        {
            Sprouted = true;
            var rand = UnityEngine.Random.Range(0, 1f);
            Growable newStemOrNode;
            if (rand < 0.1f)
            {
                newStemOrNode = StemTip();
            }
            else
            {
                newStemOrNode = NodeStemTip();
            }

            newStemOrNode.Grow(time - 2 * Plant.GrowthStepTime);
        }
    }

    // can i make this less messy?
    
    private Stem StemTip()
    {
        var stem = new Stem(this, Plant);
        Parent.Child = stem;
        Parent = stem;

        return stem;
    }

    private Node NodeStemTip()
    {
        var stem = new Stem(this, Plant);
        var node = new Node(stem, Plant);
        Parent.Child = node;
        Parent = stem;

        return node;
    }

    public override void Render(MeshData data, System.Random random, Vector3 translation, Quaternion rotation)
    {
        RenderHelper.CreateBranchSegment(data, translation, rotation, this, height: 0.2f * Height);
    }
}
